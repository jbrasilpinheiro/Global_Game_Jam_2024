using UnityEngine;
using DG.Tweening;
using TMPro;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.2f;
    private Vector3 m_initialPos;
    public static CameraShake Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        m_initialPos = transform.position;
    }
    private void Start()
    {
        ShakeCamera();
    }

    public void ShakeCamera()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength).OnComplete(() =>
        {
            transform.position = m_initialPos;
        });
    }
}
