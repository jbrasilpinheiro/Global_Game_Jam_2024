using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.2f;

    private void Start()
    {
        // Exemplo: Chame ShakeCamera() quando quiser iniciar o shake (pode ser chamado de outro script, evento, etc.)
        ShakeCamera();
    }

    public void ShakeCamera()
    {
        // Obtenha a refer�ncia para o transform da c�mera
        Transform cameraTransform = Camera.main.transform;

        // Crie uma sequ�ncia de anima��o usando o DoTween
        Sequence shakeSequence = DOTween.Sequence();

        // Adicione uma anima��o de shake usando o m�todo DOShakePosition
        shakeSequence.Append(cameraTransform.DOShakePosition(shakeDuration, shakeStrength));

        // Voc� pode adicionar mais anima��es aqui, se desejar

        // Execute a sequ�ncia
        shakeSequence.Play();
    }
}
