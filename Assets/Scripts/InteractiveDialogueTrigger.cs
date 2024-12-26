using UnityEngine;

public class InteractiveDialogueTrigger : MonoBehaviour
{
    public TextAsset dialogueFile;
    public int dialogueID;

    private DialogueUI dialogueUI;
    private DialogueDatabase dialogueDatabase;
    private DialogueData currentDialogue;

    private bool canShowDialogue = false;
    private Collider2D playerCollider = null;

    void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(dialogueFile.text);
        }
        dialogueUI.OnDialogueEnded += OnDialogueEnded;
    }

    void OnDestroy()
    {
        dialogueUI.OnDialogueEnded -= OnDialogueEnded;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogueDatabase != null)
        {
            playerCollider = other;

            canShowDialogue = true;
            dialogueUI.ShowPressXMessage();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerCollider == other)
            {
                playerCollider = null;
                canShowDialogue = false;
                dialogueUI.HidePressXMessage();
            }
        }
    }

    void Update()
    {
        if (canShowDialogue && Input.GetKeyDown(KeyCode.J) && playerCollider != null)
        {
            if (!dialogueUI.IsDialogueActive)
            {
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        foreach (var dialogue in dialogueDatabase.dialogues)
        {
            if (dialogue.id == dialogueID)
            {
                currentDialogue = dialogue;
                dialogueUI.StartDialogue(currentDialogue.lines);
                dialogueUI.HidePressXMessage();
                canShowDialogue = false;
                break;
            }
        }
    }

    private void OnDialogueEnded()
    {
       
        canShowDialogue = false;
    }
}
