using System.IO;
using UnityEngine;

public class InteractiveDialogueTrigger : MonoBehaviour
{
    public TextAsset initialDialogueFile; // Arquivo JSON do estado inicial
    private string modifiableDialoguePath; // Caminho para o JSON modificável
    public int dialogueID;

    private DialogueUI dialogueUI;
    private DialogueDatabase dialogueDatabase;
    private DialogueData currentDialogue;

    private bool canShowDialogue = false;
    private Collider2D playerCollider = null;
    void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();

        // Define o caminho para o arquivo modificável na pasta Assets/Scripts/Dialogue
        modifiableDialoguePath = Path.Combine(Application.dataPath, "Scripts", "Dialogue", "modifiableDialogue.json");

        // Exclui o arquivo existente, se houver
        if (File.Exists(modifiableDialoguePath))
        {
            File.Delete(modifiableDialoguePath);
        }

        // Cria o novo arquivo com o estado inicial
        File.WriteAllText(modifiableDialoguePath, initialDialogueFile.text);

        // Carregar o estado atual dos diálogos
        LoadModifiableDialogue();
    }


    void Update()
    {
        if (canShowDialogue && Input.GetKeyDown(KeyCode.X) && playerCollider != null)
        {
            if (!dialogueUI.IsDialogueActive)
            {
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        LoadModifiableDialogue(); // Recarregar o estado atual antes de iniciar

        // Encontra o diálogo que será iniciado
        foreach (var dialogue in dialogueDatabase.dialogues)
        {
            if (dialogue.id == dialogueID)
            {
                if (dialogue.status == 1) // Verifica se o diálogo está ativo
                {
                    // Altera o status do diálogo para o próximo valor
                    dialogue.status = dialogue.next_status;

                    // Salva o novo estado no arquivo modificável
                    SaveModifiableDialogue();

                    // Inicia o diálogo
                    currentDialogue = dialogue;
                    dialogueUI.StartDialogue(currentDialogue.lines);
                    dialogueUI.HidePressXMessage();
                    canShowDialogue = false;
                    break;
                }
                else
                {
                    dialogueUI.HidePressXMessage();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadModifiableDialogue(); // Certifique-se de carregar o estado mais recente

            foreach (var dialogue in dialogueDatabase.dialogues)
            {
                if (dialogue.id == dialogueID && dialogue.status == 1)
                {
                    playerCollider = other;
                    canShowDialogue = true;
                    dialogueUI.ShowPressXMessage();
                    return;
                }
            }
            dialogueUI.HidePressXMessage();
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

    private void LoadModifiableDialogue()
    {
        if (File.Exists(modifiableDialoguePath))
        {
            string jsonContent = File.ReadAllText(modifiableDialoguePath);
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(jsonContent);
        }
    }

    private void SaveModifiableDialogue()
    {
        string jsonContent = JsonUtility.ToJson(dialogueDatabase, true);
        File.WriteAllText(modifiableDialoguePath, jsonContent);
    }

    // Método para restaurar o estado inicial
    public void RestoreInitialState()
    {
        if (initialDialogueFile != null)
        {
            File.WriteAllText(modifiableDialoguePath, initialDialogueFile.text);
            LoadModifiableDialogue();
        }
    }
}
