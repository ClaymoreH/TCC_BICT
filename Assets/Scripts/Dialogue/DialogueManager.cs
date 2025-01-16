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
    private GameObject callingObject;  // Definindo a variável para armazenar o objeto que chama

    void Start()
    {
        dialogueUI.InitializeUI();
        playerAnimator = playerController?.GetComponent<Animator>();
    }

public void StartDialogue(string[] dialogueLines, DialogueChoice[] dialogueChoices = null, GameObject callingObject = null)
{
    ResetDialogue();
    lines = ProcessLines(dialogueLines);
    choices = dialogueChoices;
    currentLineIndex = 0;
    dialogueUI.dialogueObject.SetActive(true);
    dialogueUI.HidePressXMessage();
    dialogueUI.ShowContinueMessage();
    TogglePlayerControl(false);
    isDialogueActive = true;

    // Agora você pode fazer algo com o callingObject, como armazená-lo
    this.callingObject = callingObject;
    Debug.Log("Objeto '" + callingObject + "' Start");

    StartCoroutine(TypeCurrentLine());
}


    private void ResetDialogue()
    {
        dialogueUI.HideContinueMessage();
        dialogueUI.dialogueText.text = string.Empty;
        isTyping = false;
    }

    public IEnumerator TypeCurrentLine()
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
public void AdvanceDialogue()
{
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
            if (choices != null && choices.Length > 0)
            {
                ShowChoices();
            }
            else
            {
                EndDialogue();
            }
        }
    }
}


private void ShowChoices()
{
    isShowingChoices = true;  // As escolhas estão sendo mostradas
    foreach (var choice in choices)
    {
        dialogueUI.AddChoiceText(choice.choiceText, () => HandleChoice(choice));
    }

    dialogueUI.ShowChoices(); // Ativa a navegação e seleção das escolhas
}

private void HandleChoice(DialogueChoice choice)
{
    // Buscar o ChoiceDialogueTrigger apenas no callingObject
    ChoiceDialogueTrigger choiceTrigger = callingObject.GetComponent<ChoiceDialogueTrigger>();
    Debug.Log("Objeto '" + callingObject + "' Handle");


    if (choiceTrigger != null)
    {
        // Exibe as linhas de resposta antes de realizar a ação
        if (choice.responseLines != null && choice.responseLines.Length > 0)
        {
            StartCoroutine(ShowResponseAndExecute(choice));
        }
        else
        {
            ExecuteChoiceAction(choice, choiceTrigger);
        }
    }
    else
    {
        Debug.LogWarning("ChoiceDialogueTrigger não encontrado no callingObject");
    }
}


private IEnumerator ShowResponseAndExecute(DialogueChoice choice)
{
    DestroyChoiceButtons();

    foreach (string responseLine in choice.responseLines)
    {
        // Processa a linha antes de exibi-la
        string processedLine = dialogueUI.ProcessLine(responseLine);

        dialogueUI.dialogueText.text = ""; // Limpa o texto atual
        var typingCoroutine = StartCoroutine(dialogueUI.TypeText(processedLine)); // Inicia a digitação
        bool isTyping = true; // Indica que a digitação está em andamento

        // Aguarda o jogador pressionar X ou o término da digitação
        while (isTyping)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                StopCoroutine(typingCoroutine); // Interrompe a digitação
                dialogueUI.dialogueText.text = processedLine; // Exibe o texto completo
                isTyping = false; // Finaliza o estado de digitação
            }
            yield return null;
        }

        // Aguarda o jogador pressionar X novamente para continuar
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
    }

    // Após exibir todas as respostas, executa a ação associada
    ChoiceDialogueTrigger choiceTrigger = FindObjectOfType<ChoiceDialogueTrigger>();
    ExecuteChoiceAction(choice, choiceTrigger);
}

// Método separado para executar a ação
private void ExecuteChoiceAction(DialogueChoice choice, ChoiceDialogueTrigger choiceTrigger)
{
    switch (choice.actionType)
    {
        case "EndDialogue":
            EndDialogue();
            break;

        case "CollectItem":
            EndDialogue();
            choiceTrigger?.CollectItem(choice.ActionID);
            Destroy(callingObject); // Destrói o objeto que iniciou a interação
            break;

        case "InteractWithObject":
            choiceTrigger?.InteractWithObject(choice.ActionID);
            Destroy(callingObject); // Destrói o objeto que iniciou a interação
            EndDialogue();
            break;

        case "DeliverItem":
            choiceTrigger?.DeliverItem(choice.ActionID);
            EndDialogue();
            break;

        case "OpenPuzzle":
            choiceTrigger?.OpenPuzzle(choice.ActionID);
            Destroy(callingObject); // Destrói o objeto que iniciou a interação
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
    dialogueUI.choiceTextList.Clear();  // Limpa a lista de escolhas para não deixar rastros

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
