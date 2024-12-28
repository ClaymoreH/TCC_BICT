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
        LoadDialogueDatabase();  // Carrega o banco de dados de diálogos na inicialização
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogueDatabase != null && !hasTriggered)
        {
            LoadDialogueDatabase();  // Recarrega o banco de dados sempre que o jogador entra no gatilho

            foreach (var dialogue in dialogueDatabase.dialogues)
            {
                if (dialogue.id == dialogueID && dialogue.status == 1)
                {
                    hasTriggered = true;
                    StartDialogue();
                    return;
                }
            }
        }
    }

    void LoadDialogueDatabase()
    {
        if (dialogueFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(dialogueFile.text);
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


