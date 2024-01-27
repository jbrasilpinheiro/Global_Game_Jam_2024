using Fusion;
using Fusion.Addons.Physics;
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

    private void Start()
    {
        SpawnAtPosition(Vector3.zero);
    }

    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out InputData data))
        {
            m_rigidBody2D.Rigidbody.velocity = data.direction.normalized * m_speed;
        }
    }

    public void SpawnAtPosition(Vector3 position)
    {
        m_rigidBody2D.Rigidbody.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COllided");
    }

}
