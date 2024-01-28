using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCheckCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCharacter"))
        {
            Debug.Log("Collided");
            int life = collision.GetComponent<PlayerManager>().Life -= 1;
            collision.GetComponent<PlayerManager>().TakeDamage(life);
            collision.GetComponent<PlayerManager>().Bounce = true;
            collision.GetComponent<PlayerManager>().AttackerPos = transform.parent.parent.parent.position;
            AudioManager.Instance.PlayEffect(1);
        }
    }
}
