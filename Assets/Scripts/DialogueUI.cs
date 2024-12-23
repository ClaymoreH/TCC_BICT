using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public Image dialogueImage; // Imagem de fundo do di�logo
    public TMP_Text dialogueText; // Texto principal do di�logo
    public Image additionalImage; // Imagem adicional
    public TMP_Text additionalText; // Texto adicional
    public GameObject dialogueObject; // Objeto contendo a UI do di�logo
    public TMP_Text continueText; // Texto "Pressione X para continuar" piscando
    public float typingSpeed = 0.05f; // Velocidade de digita��o
    public float blinkSpeed = 0.5f; // Velocidade de piscagem do texto "Pressione X"

    // Elemento adicional para "Pressione X"
    public Image pressXImage; // Imagem que servir� de fundo para o texto "Pressione X"
    public TMP_Text pressXText; // Texto "Pressione X para interagir"

    private string[] lines; // Linhas do di�logo
    private int currentLineIndex = 0; // �ndice da linha atual
    private bool isTyping = false; // Verifica se est� digitando
    private bool isDialogueActive = false; // Verifica se o di�logo est� ativo
    private Coroutine blinkCoroutine; // Refer�ncia para a coroutine de piscagem do texto

    private PlayerController playerController; // Refer�ncia ao controlador do jogador
    private Animator playerAnimator; // Refer�ncia ao Animator do jogador

    [Header("Texto Adicional")]
    [TextArea] public string additionalTextContent = "Texto adicional"; // Texto adicional (exposto no Inspector)

    // Evento para notificar o fim do di�logo
    public event System.Action OnDialogueEnded;

    void Start()
    {
        dialogueObject.SetActive(false);

        playerController = FindObjectOfType<PlayerController>();
        playerAnimator = playerController != null ? playerController.GetComponent<Animator>() : null;

        if (additionalImage != null)
        {
            additionalImage.enabled = false; // Garante que a nova imagem esteja desativada no in�cio
        }

        if (additionalText != null)
        {
            additionalText.text = ""; // Garante que o novo texto esteja vazio no in�cio
        }

        if (pressXImage != null)
        {
            pressXImage.gameObject.SetActive(false); // Esconde a imagem de fundo de "Pressione X"
        }
    }

    public void ShowPressXMessage()
    {
        if (pressXImage != null)
        {
            pressXImage.gameObject.SetActive(true); // Mostra a mensagem "Pressione X"
        }
    }

    public void HidePressXMessage()
    {
        if (pressXImage != null)
        {
            pressXImage.gameObject.SetActive(false); // Esconde a mensagem "Pressione X"
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        lines = dialogueLines;
        currentLineIndex = 0;

        dialogueObject.SetActive(true);
        dialogueImage.enabled = true;
        dialogueText.text = "";
        continueText.gameObject.SetActive(false);

        if (additionalImage != null)
        {
            additionalImage.enabled = true; // Ativa a nova imagem
        }

        if (additionalText != null)
        {
            additionalText.text = additionalTextContent; // Define o texto adicional, usando o valor do Inspector
        }

        if (playerController != null)
        {
            playerController.enabled = false;
            if (playerAnimator != null)
            {
                playerAnimator.enabled = false;
            }
        }

        isDialogueActive = true;
        StartCoroutine(TypeText(lines[currentLineIndex]));
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = ""; // Limpa o texto antes de come�ar
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter; // Adiciona letra por letra
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        // Ap�s digitar a linha, ativa o texto de "Pressione X para continuar"
        continueText.gameObject.SetActive(true);
        blinkCoroutine = StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            continueText.alpha = 1f;
            yield return new WaitForSeconds(blinkSpeed);
            continueText.alpha = 0f;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.X) && !isTyping)
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        if (isTyping) return;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        continueText.gameObject.SetActive(false);

        currentLineIndex++;
        if (currentLineIndex < lines.Length)
        {
            StartCoroutine(TypeText(lines[currentLineIndex]));
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueImage.enabled = false;
        dialogueObject.SetActive(false);
        dialogueText.text = "";

        if (additionalImage != null)
        {
            additionalImage.enabled = false; // Desativa a nova imagem
        }

        if (additionalText != null)
        {
            additionalText.text = ""; // Limpa o texto adicional
        }

        if (playerController != null)
        {
            playerController.enabled = true;
            if (playerAnimator != null)
            {
                playerAnimator.enabled = true;
            }
        }

        isDialogueActive = false;

        // Notifica o fim do di�logo
        OnDialogueEnded?.Invoke();
    }
}