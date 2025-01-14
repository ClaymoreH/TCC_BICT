using UnityEngine;
using UnityEngine.UI;

public class PanelFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // Referência ao CanvasGroup do painel
    [SerializeField] private float fadeDuration = 0.5f; // Duração do fade in/out em segundos

    // Inicia o fade in
    public void FadeIn()
    {
        gameObject.SetActive(true); // Ativa o painel
        StartCoroutine(Fade(0f, 1f)); // Faz o fade de 0 para 1
    }

    // Inicia o fade out
    public void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f, () =>
        {
            gameObject.SetActive(false); // Desativa o painel após o fade
        }));
    }

    // Corrotina que realiza o efeito de fade
    private System.Collections.IEnumerator Fade(float startAlpha, float endAlpha, System.Action onComplete = null)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Garante que o alpha final seja o desejado
        onComplete?.Invoke(); // Chama a função onComplete, se existir
    }
}
