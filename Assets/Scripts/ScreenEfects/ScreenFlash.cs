using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage; // Painel da UI para o flash
    public float flashDuration = 0.5f; // Duração total do efeito
    public float maxAlpha = 0.6f; // Transparência máxima (60%)

    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            // Calcula o valor do alpha com limite máximo
            float alpha = Mathf.PingPong(elapsed * 2 / flashDuration, maxAlpha);
            flashImage.color = new Color(1, 0, 0, alpha); // Define a cor do flash
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reseta a cor do flash para completamente transparente
        flashImage.color = new Color(1, 0, 0, 0);
    }
}
