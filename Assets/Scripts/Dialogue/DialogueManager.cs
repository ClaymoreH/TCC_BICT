using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    public DialogueUI dialogueUI;
    public PlayerController playerController;

    private string[] lines;
    private int currentLineIndex;
    private bool isTyping;
    private bool isDialogueActive;
    private Animator playerAnimator;
    private DialogueChoice[] choices;  // Novo campo para armazenar as escolhas do diálogo

    public event System.Action OnDialogueEnded;
    public bool IsDialogueActive => isDialogueActive;
    public bool isShowingChoices = false;  // As escolhas estão sendo mostradas

    void Start()
    {
        dialogueUI.InitializeUI();
        playerAnimator = playerController?.GetComponent<Animator>();
    }

    public void StartDialogue(string[] dialogueLines, DialogueChoice[] dialogueChoices = null)
    {
        ResetDialogue();
        lines = ProcessLines(dialogueLines);
        choices = dialogueChoices;  // Armazena as escolhas, se houver
        currentLineIndex = 0;
        dialogueUI.dialogueObject.SetActive(true);
        dialogueUI.HidePressXMessage();
        dialogueUI.ShowContinueMessage();
        TogglePlayerControl(false);
        isDialogueActive = true;
        StartCoroutine(TypeCurrentLine());
    }

    private void ResetDialogue()
    {
        dialogueUI.HideContinueMessage();
        dialogueUI.dialogueText.text = string.Empty;
        isTyping = false;
    }

    private IEnumerator TypeCurrentLine()
    {
        isTyping = true;
        yield return StartCoroutine(dialogueUI.TypeText(lines[currentLineIndex]));
        isTyping = false;
        dialogueUI.ShowContinueMessage();
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.X))
            AdvanceDialogue();
    }
private void AdvanceDialogue()
{
    // Impede que o jogador avance o diálogo enquanto as escolhas estão visíveis
    if (isShowingChoices)
        return;

    if (isTyping)
    {
        StopAllCoroutines();
        dialogueUI.dialogueText.text = lines[currentLineIndex];
        isTyping = false;
        dialogueUI.ShowContinueMessage();
    }
    else
    {
        currentLineIndex++;
        if (currentLineIndex < lines.Length)
        {
            StartCoroutine(TypeCurrentLine());
        }
        else
        {
            // Mostrar as escolhas imediatamente após a última linha
            ShowChoices();
        }
    }
}

private void ShowChoices()
{
    isShowingChoices = true;  // As escolhas estão sendo mostradas
    DestroyChoiceTextObjects();
    foreach (var choice in choices)
    {
        dialogueUI.AddChoiceText(choice.choiceText, () => HandleChoice(choice));
    }

    dialogueUI.ShowChoices(); // Ativa a navegação e seleção das escolhas
}

// Função para destruir os objetos de texto das escolhas anteriores
private void DestroyChoiceTextObjects()
{
    foreach (Transform child in dialogueUI.choicesContainer.transform)
    {
        Destroy(child.gameObject);  // Remove a escolha do painel
    }

    // Limpa a lista que armazena as escolhas
    dialogueUI.choiceTextList.Clear();  // Limpa a lista de escolhas para não deixar rastros
}

    private void HandleChoice(DialogueChoice choice)
    {
        // Acesso ao ChoiceDialogueTrigger
        ChoiceDialogueTrigger choiceTrigger = FindObjectOfType<ChoiceDialogueTrigger>();

        // Lógica para lidar com a escolha (conforme suas ações definidas)
        switch (choice.actionType)
        {
            case "EndDialogue":
                EndDialogue();
                break;

            case "CollectItem":
                choiceTrigger?.CollectItem(choice.ActionID);  // Chama a função do ChoiceDialogueTrigger
                EndDialogue();
                break;

            case "InteractWithObject":
                choiceTrigger?.InteractWithObject(choice.ActionID);  // Chama a função do ChoiceDialogueTrigger
                EndDialogue();
                break;

            case "DeliverItem":
                choiceTrigger?.DeliverItem(choice.ActionID);  // Chama a função do ChoiceDialogueTrigger
                EndDialogue();
                break;

            case "OpenPuzzle":
                choiceTrigger?.OpenPuzzle(choice.ActionID);  // Chama a função do ChoiceDialogueTrigger
                EndDialogue();
                break;

            default:
                Debug.LogWarning("Ação desconhecida: " + choice.actionType);
                break;
        }
        isShowingChoices = false;

    }


private void EndDialogue()
{
    // Destrói os botões de escolha se existirem
    DestroyChoiceButtons();

    // Desativa o diálogo
    dialogueUI.dialogueObject.SetActive(false);
    dialogueUI.HideContinueMessage();
    TogglePlayerControl(true);
    isDialogueActive = false;
    isShowingChoices = false;  // Reseta o estado das escolhas
    OnDialogueEnded?.Invoke();
}


private void DestroyChoiceButtons()
{
    // Destrói todos os botões dentro do choicesContainer
    foreach (Transform child in dialogueUI.choicesContainer.transform)
    {
        Destroy(child.gameObject);
    }
}

    private void TogglePlayerControl(bool state)
    {
        if (playerController != null) playerController.enabled = state;
        if (playerAnimator != null) playerAnimator.enabled = state;
    }

    private string[] ProcessLines(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
            lines[i] = dialogueUI.ProcessLine(lines[i]);
        return lines;
    }
}
