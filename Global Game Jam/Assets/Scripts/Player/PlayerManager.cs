using BarthaSzabolcs.Tutorial_SpriteFlash;
using Fusion;
using System.Collections;
using UnityEngine;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;


public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    private NetworkRigidbody2D m_rigidBody2D;
    [SerializeField]
    private float m_speed = 2;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private Transform m_spriteTransform;
    [SerializeField]
    private Transform m_weaponLook;
    [SerializeField]
    private Animator m_attackAnimator;
    [SerializeField]
    private Collider2D m_gloveCollider;
    [SerializeField]
    private SimpleFlash m_flash;
    [Networked]
    public NetworkButtons PrevButtons { get; set; }
    private float m_cooldownTreshold = 2.5f;
    private float m_attackBufferTime = 0;
    public bool Bounce;

    [Networked]
    public int SkinIndex { get; set; }
    [Networked]
    public int PlayerID { get; set; }
    [Networked]
    public int PlayerDir { get; set; }
    [Networked]
    public bool PlayerRunning { get; set; }
    [Networked]
    public int Life { get; set; }
    [Networked]
    public Vector3 MouseLook { get; set; }
    [Networked]
    public Vector3 AttackerPos { get; set; }

    private ChangeDetector _changeDetector;
    public SpriteRenderer HeadSprite;
    private Coroutine m_bounceRoutine;

    private void Start()
    {
        SetupCharacter(Vector3.zero);
    }

    public override void FixedUpdateNetwork() 
    {
        if (GetInput<InputData>(out InputData data))
        {
            var pressed = data.GetButtonPressed(PrevButtons);
            PrevButtons = data.Buttons;
            m_rigidBody2D.Rigidbody.velocity = data.direction.normalized * m_speed;
            SetMouseLookRotation(GetMouseLookRotation(data.mouseDir));
            Attack(pressed);
        }

        if (Bounce)
        {
            m_rigidBody2D.Rigidbody.AddForce(-AttackerPos.normalized * 100000 * Time.deltaTime);
            //DoBounceMoveFromAttack(AttackerPos);
            if(m_bounceRoutine == null)
                m_bounceRoutine = StartCoroutine(BounceRoutine());
        }

        if (m_rigidBody2D.Rigidbody.velocity.x > 0)
        {
            m_spriteTransform.localScale = new Vector3(5, 5);
            PlayerDir = 0;
        }
        else if (m_rigidBody2D.Rigidbody.velocity.x < 0)
        {
            m_spriteTransform.localScale = new Vector3(-5, 5);
            PlayerDir = 1;
        }

        if (m_rigidBody2D.Rigidbody.velocity.magnitude > 0)
        {
            m_animator.SetBool("IsRunning", true);
            PlayerRunning = true;
        }
        else
        {
            m_animator.SetBool("IsRunning", false);
            PlayerRunning = false;
        }
    }

    private void Attack(NetworkButtons pressedButtons)
    {
        if (pressedButtons.IsSet(InputButton.ATTACK) && CalculateAttackBuffer())
        {
            m_attackBufferTime = Runner.SimulationTime;
            DoAttackAnimation();
            StartCoroutine(DoColliderEnableRoutine());
        }
    }

    public IEnumerator DoColliderEnableRoutine()
    {
        m_gloveCollider.enabled = true;
        yield return new WaitForEndOfFrame();
        m_gloveCollider.enabled = false;
    }

    public void DoAttackAnimation()
    {
        m_attackAnimator.SetTrigger("Hit");
    }

    public IEnumerator BounceRoutine()
    {
        yield return new WaitForSeconds(.5f);
        Bounce = false;
        m_bounceRoutine = null;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_Attack()
    {
        //SkinIndex = index;
    }

    public bool CalculateAttackBuffer()
    {
        return (Runner.SimulationTime >= (m_cooldownTreshold + m_attackBufferTime));
    }

    public void SetMouseLookRotation(Vector3 dir)
    {
        m_weaponLook.transform.eulerAngles = dir;
    }

    public Vector3 GetMouseLookRotation(Vector3 mouseWorldPosition)
    {
        Vector2 directionToMouse = (Vector2)mouseWorldPosition - m_rigidBody2D.Rigidbody.position;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        return rotation.eulerAngles;
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(SkinIndex):
                    OnPlayerColourChanged(SkinIndex);
                    break;
                case nameof(PlayerRunning):
                    OnPlayerAnimationChanged(PlayerRunning);
                    break;
                case nameof(PlayerDir):
                    OnPlayerDirectionChanged(PlayerDir);
                    break;
                case nameof(PrevButtons):
                    Attack(PrevButtons);
                    break;
                case nameof(Life):
                    TakeDamage(Life);
                    break;
                /*case nameof(AttackerPos):
                    DoBounceMoveFromAttack(AttackerPos);                 
                    break;*/
            }
        }
    }

    public void DoBounceMoveFromAttack(Vector3 damagerPos)
    {
        //m_rigidBody2D.Rigidbody.AddForce(-damagerPos.normalized * 100, ForceMode2D.Impulse);
    }

    public void TakeDamage(int life)
    {
        m_flash.Flash();
        if (life <= 0)
        {
            
        }
    }

    public void OnPlayerColourChanged(int skinIndex)
    {
        HeadSprite.sprite = Resources.Load<Sprite>("CharacterSelection/" + skinIndex.ToString());
    }

    public void OnPlayerAnimationChanged(bool isRunning)
    {
        m_animator.SetBool("IsRunning", isRunning);
    }

    public void OnPlayerDirectionChanged(int dir)
    {
        if (dir == 0)
        {
            m_spriteTransform.localScale = new Vector3(5, 5);
        }
        else if (dir == 1)
        {
            m_spriteTransform.localScale = new Vector3(-5, 5);
        }
    }

    public void SetupCharacter(Vector3 position)
    {
        m_rigidBody2D.Rigidbody.position = position;
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        PlayerID = Object.InputAuthority.PlayerId;
        if (Object.HasInputAuthority)
        {
            RPC_SetSkin(MultiplayerManager.Instance.SelectedSkin);
        }
        HeadSprite.sprite = Resources.Load<Sprite>("CharacterSelection/" + SkinIndex.ToString());
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetSkin(int index)
    {
        SkinIndex = index;
    }

}
