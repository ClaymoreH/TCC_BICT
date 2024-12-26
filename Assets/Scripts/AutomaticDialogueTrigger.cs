using UnityEngine;

public class AutomaticDialogueTrigger : MonoBehaviour
{
    public TextAsset dialogueFile;
    public int dialogueID;

    private DialogueUI dialogueUI;
    private DialogueDatabase dialogueDatabase;

    private bool hasTriggered = false;

    void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
        if (dialogueFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(dialogueFile.text);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogueDatabase != null && !hasTriggered)
        {
            hasTriggered = true;
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        foreach (var dialogue in dialogueDatabase.dialogues)
        {
            if (dialogue.id == dialogueID)
            {
                dialogueUI.StartDialogue(dialogue.lines);
                dialogueUI.HidePressXMessage();
                break;
            }
        }
    }
}


