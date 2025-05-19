using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LuzComFadeClaro : MonoBehaviour
{
    [Header("Luzes")]
    public Light2D luzAmbiente;
    public Light2D luzDoJogador;

    [Header("Fade Branco")]
    public Image fadeOverlay;         // UI Image branca cobrindo a tela
    public float tempoDoFade = 2f;    // Duração total do fade
    public float maxAlpha = 0.25f;    // Intensidade máxima do fade (0 = invisível, 1 = branco total)

    private Coroutine fadeCoroutine;

    // Chama isso pra simular o amanhecer
    public void RemoverEscuridao()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeClarear());
    }

    private IEnumerator FadeClarear()
    {
        // Garante que o overlay está ativo
        fadeOverlay.gameObject.SetActive(true);

        // Começa transparente
        Color cor = fadeOverlay.color;
        cor.a = 0f;
        fadeOverlay.color = cor;

        // Parte 1: Aumenta o alpha até maxAlpha
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (tempoDoFade / 2f);
            cor.a = Mathf.Lerp(0f, maxAlpha, t);
            fadeOverlay.color = cor;
            yield return null;
        }

        // Parte 2: Desliga as luzes
        if (luzAmbiente != null) luzAmbiente.enabled = false;
        if (luzDoJogador != null) luzDoJogador.enabled = false;

        // Parte 3: Diminui o alpha de volta para 0
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (tempoDoFade / 2f);
            cor.a = Mathf.Lerp(maxAlpha, 0f, t);
            fadeOverlay.color = cor;
            yield return null;
        }

        // Desativa o overlay ao final
        fadeOverlay.gameObject.SetActive(false);
    }
}
