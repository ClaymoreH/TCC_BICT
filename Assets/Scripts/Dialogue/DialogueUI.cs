using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    public Image dialogueImage;
    public TMP_Text dialogueText;
    public GameObject dialogueObject;
    public TMP_Text continueText;
    public float typingSpeed = 0.05f;
    public float blinkSpeed = 0.5f;

    public Image pressXImage;
    public TMP_Text pressXText;

    private string[] lines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private Coroutine blinkCoroutine;

    private PlayerController playerController;
    private Animator playerAnimator;

    public event System.Action OnDialogueEnded;

    public bool IsDialogueActive => isDialogueActive;

    private readonly Dictionary<char, string> colorMap = new Dictionary<char, string>
    {
        { '#', "red" },
        { '@', "blue" },
        { '&', "green" }
    };

    // Cont�iner de escolhas e o prefab do bot�o
    public Transform choicesContainer; // O painel ou cont�iner onde os bot�es de escolha ser�o instanciados
    public GameObject choiceButtonPrefab; // O prefab do bot�o de escolha

    void Start()
    {
        dialogueObject.SetActive(false);
        playerController = FindObjectOfType<PlayerController>();
        playerAnimator = playerController?.GetComponent<Animator>();

        SetUIElementState(pressXImage, false);
        SetUIElementState(pressXText, false);
    }

    private void SetUIElementState(Graphic element, bool state)
    {
        if (element != null) element.enabled = state;
    }

    public void ShowPressXMessage()
    {
        SetUIElementState(pressXImage, true);
        SetUIElementState(pressXText, true);
    }

    public void HidePressXMessage()
    {
        SetUIElementState(pressXImage, false);
        SetUIElementState(pressXText, false);
    }

    public void ShowContinueMessage()
    {
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
            if (blinkCoroutine == null)
                blinkCoroutine = StartCoroutine(BlinkText());
        }
    }

    public void HideContinueMessage()
    {
        if (continueText != null)
        {
            continueText.gameObject.SetActive(false);
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        ResetDialogueState();

        lines = new string[dialogueLines.Length];
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            lines[i] = ProcessLine(dialogueLines[i]);
        }
        currentLineIndex = 0;

        dialogueObject.SetActive(true);
        dialogueImage.enabled = true;
        dialogueText.text = "";
        HidePressXMessage();

        if (playerController != null) TogglePlayerControl(false);
        isDialogueActive = true;

        StartCoroutine(TypeText(lines[currentLineIndex]));
    }

    private void ResetDialogueState()
    {
        HideContinueMessage();
        dialogueText.text = string.Empty;
        isTyping = false;
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = string.Empty;

        int charIndex = 0;
        while (charIndex < line.Length)
        {
            if (line[charIndex] == '<')
            {
                int closingIndex = line.IndexOf('>', charIndex);
                if (closingIndex != -1)
                {
                    dialogueText.text += line.Substring(charIndex, closingIndex - charIndex + 1);
                    charIndex = closingIndex + 1;
                }
                else
                {
                    dialogueText.text += line[charIndex];
                    charIndex++;
                }
            }
            else
            {
                dialogueText.text += line[charIndex];
                charIndex++;
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowContinueMessage();
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

    void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = lines[currentLineIndex];
            isTyping = false;
            ShowContinueMessage();
        }
        else
        {
            HideContinueMessage();

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
    }

    private void EndDialogue()
    {
        dialogueImage.enabled = false;
        dialogueObject.SetActive(false);
        dialogueText.text = string.Empty;

        HideContinueMessage();

        if (playerController != null) TogglePlayerControl(true);
        isDialogueActive = false;
        OnDialogueEnded?.Invoke();
    }

    private void TogglePlayerControl(bool state)
    {
        playerController.enabled = state;
        if (playerAnimator != null) playerAnimator.enabled = state;
    }

    private string ProcessLine(string line)
    {
        string processedLine = "";
        string[] words = line.Split(' ');

        foreach (string word in words)
        {
            if (word.Length > 0 && colorMap.ContainsKey(word[0]))
            {
                char specialChar = word[0];
                string color = colorMap[specialChar];
                processedLine += $"<color={color}>{word.Substring(1)}</color> ";
            }
            else
            {
                processedLine += word + " ";
            }
        }

        return processedLine.Trim();
    }

    // M�todo para exibir as escolhas
    public void SetChoices(string[] choices, System.Action<int> onChoiceSelected)
    {
        // Verifica se o prefab do bot�o de escolha est� atribu�do
        if (choiceButtonPrefab == null)
        {
            Debug.LogError("Choice button prefab is missing!");
            return;
        }

        // Limpa as escolhas anteriores
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < choices.Length; i++)
        {
            var choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            var buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = choices[i];
            }

            int index = i; // Armazena o �ndice da escolha
            choiceButton.GetComponent<Button>().onClick.AddListener(() => onChoiceSelected(index)); // Lida com o clique
        }
    }
}
