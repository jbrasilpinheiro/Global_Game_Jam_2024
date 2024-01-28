using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource m_AudioSource;
    [SerializeField]
    private AudioClip[] m_audios;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayEffect(int index)
    {
        //m_AudioSource.clip = m_audios[index];
        m_AudioSource.PlayOneShot(m_audios[index]);
    }
}
