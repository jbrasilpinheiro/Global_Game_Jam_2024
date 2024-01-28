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
        // Obtenha a referência para o transform da câmera
        Transform cameraTransform = Camera.main.transform;

        // Crie uma sequência de animação usando o DoTween
        Sequence shakeSequence = DOTween.Sequence();

        // Adicione uma animação de shake usando o método DOShakePosition
        shakeSequence.Append(cameraTransform.DOShakePosition(shakeDuration, shakeStrength));

        // Você pode adicionar mais animações aqui, se desejar

        // Execute a sequência
        shakeSequence.Play();
    }
}
