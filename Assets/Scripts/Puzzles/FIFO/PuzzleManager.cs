using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    [Header("Configurações do Puzzle")]
    public Button confirmButton;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip successSound;
    public AudioClip errorSound;

    [Header("UI de Feedback")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 1f;

    public void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ValidarPuzzle);
        }

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }

    // Função para exibir feedback com som
    public void ExibirFeedback(string mensagem, AudioClip som)
    {
        if (feedbackText != null)
        {
            feedbackText.text = mensagem;
        }

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
        }

        if (audioSource != null && som != null)
        {
            audioSource.PlayOneShot(som);
        }

        StartCoroutine(EsconderFeedback());
    }

    private IEnumerator EsconderFeedback()
    {
        yield return new WaitForSeconds(feedbackDuration);
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }

    // Função genérica de validação, chamada pelos scripts específicos
    public virtual void ValidarPuzzle()
    {
        // Este método pode ser sobrescrito nos scripts específicos
    }
}
