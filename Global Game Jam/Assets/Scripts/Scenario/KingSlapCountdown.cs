using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlapCountdown : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator;

    private void Start()
    {
        StartCoroutine(PlayEverySeconds(5));
    }

    public IEnumerator PlayEverySeconds(int seconds)
    {
        while (true)
        {
            m_animator.SetTrigger("Slap");
            yield return new WaitForSeconds(seconds);
        }
    }
}
