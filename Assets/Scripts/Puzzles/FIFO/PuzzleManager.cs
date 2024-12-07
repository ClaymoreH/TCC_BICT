using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importação necessária para TextMeshPro

public class PuzzleManager : MonoBehaviour
{
    [Header("Configurações do Puzzle")]
    public List<Transform> slots; // Lista dos slots para validação
    public Button confirmButton;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip successSound;
    public AudioClip errorSound;

    [Header("UI de Feedback")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText; 
    public float feedbackDuration = 1f;

    private void Start()
    {
        // Botão para chamar o método de validação
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ValidarPuzzle);
        }

        // Esconde o painel de feedback no início
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }

    public void ValidarPuzzle()
    {
        // Inicia a corrotina para tocar os sons e exibir o feedback
        StartCoroutine(PlayValidationSounds());
    }

    private IEnumerator PlayValidationSounds()
    {
        // Tocar som de clique
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Espera o tempo do som de clique terminar antes de continuar
        yield return new WaitForSeconds(clickSound.length);

        List<PuzzleObjectData> objectsInSlots = new List<PuzzleObjectData>();

        // Itera pelos slots e coleta os dados dos objetos dentro deles
        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                PuzzleObjectData objectData = slot.GetChild(0).GetComponent<PuzzleObjectData>();
                if (objectData != null)
                {
                    objectsInSlots.Add(objectData);
                }
                else
                {
                    ExibirFeedback("ERRO", errorSound);
                    yield break; // Retorna da corrotina
                }
            }
            else
            {
                ExibirFeedback("ERRO", errorSound);
                yield break; // Retorna da corrotina
            }
        }

        // Validação FIFO
        for (int i = 0; i < objectsInSlots.Count - 1; i++)
        {
            try
            {
                // Converte Data e Hora para DateTime para comparar
                System.DateTime currentDateTime = System.DateTime.Parse($"{objectsInSlots[i].data} {objectsInSlots[i].hora}");
                System.DateTime nextDateTime = System.DateTime.Parse($"{objectsInSlots[i + 1].data} {objectsInSlots[i + 1].hora}");

                if (currentDateTime > nextDateTime)
                {
                    ExibirFeedback("ERRO", errorSound);
                    yield break; // Retorna da corrotina
                }
            }
            catch (System.FormatException)
            {
                ExibirFeedback("ERRO", errorSound);
                yield break; // Retorna da corrotina
            }
        }

        // Se chegou rodou ate aqui, a ordem tá correta
        ExibirFeedback("Sucesso!", successSound);
    }

    private void ExibirFeedback(string mensagem, AudioClip som)
    {
        // Atualizar o texto de feedback
        if (feedbackText != null)
        {
            feedbackText.text = mensagem; // Altera o texto do feedback
        }

        // Mostrar o painel de feedback
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
        }

        // Tocar o som correspondente
        if (audioSource != null && som != null)
        {
            audioSource.PlayOneShot(som);
        }

        // Esconde o painel após a duração configurada
        StartCoroutine(EsconderFeedback());
    }

    private IEnumerator EsconderFeedback()
    {
        yield return new WaitForSeconds(feedbackDuration);

        // Esconde o painel de feedback
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
}
