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
    [Networked]
    public int SkinIndex { get; set; }
    [Networked]
    public int PlayerID { get; set; }
    [Networked]
    public int PlayerDir { get; set; }
    [Networked]
    public bool PlayerRunning { get; set; }
    private ChangeDetector _changeDetector;
    public SpriteRenderer HeadSprite;

    private void Start()
    {
        SetupCharacter(Vector3.zero);
        StartCoroutine(SkindIndexRoutine());
    }

    public IEnumerator SkindIndexRoutine()
    {
        yield return new WaitForEndOfFrame();
        SkinIndex = MultiplayerManager.Instance.SelectedSkin;
    }

    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out InputData data))
        {
            m_rigidBody2D.Rigidbody.velocity = data.direction.normalized * m_speed;
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
