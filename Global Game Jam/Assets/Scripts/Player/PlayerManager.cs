using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject.SpaceFighter;
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
    [Networked]
    public int SkinIndex { get; set; }
    [Networked]
    public int PlayerID { get; set; }
    [Networked]
    public int PlayerDir { get; set; }
    [Networked]
    public bool PlayerRunning { get; set; }
    [Networked]
    public Vector3 MouseLook { get; set; }
    private ChangeDetector _changeDetector;
    public SpriteRenderer HeadSprite;

    private void Start()
    {
        SetupCharacter(Vector3.zero);
    }

    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out InputData data))
        {
            m_rigidBody2D.Rigidbody.velocity = data.direction.normalized * m_speed;
        }
        Vector3 mouseDir = GetMouseLookRotation(data.GetMousePosition());
        SetMouseLookRotation(mouseDir);
        //MouseLook = mouseDir;

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

    public void SetMouseLookRotation(Vector3 dir)
    {
        m_weaponLook.transform.eulerAngles = dir;
    }

    public Vector3 GetMouseLookRotation(Vector3 mouseWorldPosition)
    {
        Vector3 directionToMouse = mouseWorldPosition - m_rigidBody2D.Rigidbody.transform.position;

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
                /*case nameof(MouseLook):
                    SetMouseLookRotation(MouseLook);
                    break;*/
            }
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
