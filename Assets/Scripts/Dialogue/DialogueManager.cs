using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    public DialogueUI dialogueUI;
    public PlayerController playerController;

    private List<DialogueLine> lines;
    private int currentLineIndex;
    private bool isTyping;
    private bool isDialogueActive;
    private Animator playerAnimator;
    private DialogueChoice[] choices;

    public event System.Action OnDialogueEnded;
    public bool IsDialogueActive => isDialogueActive;
    public bool isShowingChoices = false;
    private GameObject callingObject;

    void Start()
    {
        dialogueUI.InitializeUI();
        playerAnimator = playerController?.GetComponent<Animator>();
    }

    public void StartDialogue(List<DialogueLine> dialogueLines, DialogueChoice[] dialogueChoices = null, GameObject callingObject = null)
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

        DialogueLine currentLine = lines[currentLineIndex];
        dialogueUI.speakerNameText.text = currentLine.npc == 0 ? "Helena" : "Athena";

        yield return StartCoroutine(dialogueUI.TypeText(currentLine.text));
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
            dialogueUI.dialogueText.text = lines[currentLineIndex].text;
            isTyping = false;
            dialogueUI.ShowContinueMessage();
        }
        else
        {
            currentLineIndex++;
            if (currentLineIndex < lines.Count)
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
        isShowingChoices = true;
        foreach (var choice in choices)
        {
            dialogueUI.AddChoiceText(choice.choiceText, () => HandleChoice(choice));
        }

        dialogueUI.ShowChoices();
    }

    private void HandleChoice(DialogueChoice choice)
    {
        ChoiceDialogueTrigger choiceTrigger = callingObject.GetComponent<ChoiceDialogueTrigger>();
        Debug.Log("Objeto '" + callingObject + "' Handle");

        if (choiceTrigger != null)
        {
            if (choice.responseLines != null && choice.responseLines.Count > 0)
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

        foreach (DialogueLine responseLine in choice.responseLines)
        {
            string processedLine = dialogueUI.ProcessLine(responseLine.text);
            dialogueUI.dialogueText.text = "";
            var typingCoroutine = StartCoroutine(dialogueUI.TypeText(processedLine));
            bool localTyping = true;

            while (localTyping)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    StopCoroutine(typingCoroutine);
                    dialogueUI.dialogueText.text = processedLine;
                    localTyping = false;
                }
                yield return null;
            }

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        }

        ChoiceDialogueTrigger choiceTrigger = callingObject.GetComponent<ChoiceDialogueTrigger>();
        ExecuteChoiceAction(choice, choiceTrigger);
    }

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
                Destroy(callingObject);
                break;

            case "InteractWithObject":
                choiceTrigger?.InteractWithObject(choice.ActionID);
                Destroy(callingObject);
                EndDialogue();
                break;

            case "DeliverItem":
                choiceTrigger?.DeliverItem(choice.ActionID);
                EndDialogue();
                Destroy(callingObject);
                break;

            case "OpenPuzzle":
                choiceTrigger?.OpenPuzzle(choice.ActionID);
                Destroy(callingObject);
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
        DestroyChoiceButtons();
        dialogueUI.dialogueObject.SetActive(false);
        dialogueUI.HideContinueMessage();
        TogglePlayerControl(true);
        isDialogueActive = false;
        isShowingChoices = false;
        OnDialogueEnded?.Invoke();
    }

    private void DestroyChoiceButtons()
    {
        foreach (Transform child in dialogueUI.choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        dialogueUI.choiceTextList.Clear();
    }

    private void TogglePlayerControl(bool state)
    {
        if (playerController != null) playerController.enabled = state;
        if (playerAnimator != null) playerAnimator.enabled = state;
    }

    private List<DialogueLine> ProcessLines(List<DialogueLine> rawLines)
    {
        foreach (var line in rawLines)
        {
            line.text = dialogueUI.ProcessLine(line.text);
        }
        return rawLines;
    }
}
