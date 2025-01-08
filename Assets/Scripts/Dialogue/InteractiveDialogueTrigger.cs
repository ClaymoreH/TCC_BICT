using System.IO;
using UnityEngine;

public class InteractiveDialogueTrigger : MonoBehaviour
{
    public TextAsset initialDialogueFile; // Arquivo JSON do estado inicial
    public string modifiableDialoguePath; // Caminho para o JSON modificável
    public int dialogueID;

    public DialogueManager dialogueManager;
    public DialogueDatabase dialogueDatabase;
    public DialogueData currentDialogue;

    public bool canShowDialogue = false;
    public Collider2D playerCollider = null;

    public void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

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

    public void Update()
    {
        if (canShowDialogue && Input.GetKeyDown(KeyCode.X) && playerCollider != null)
        {
            if (!dialogueManager.IsDialogueActive)
            {
                StartDialogue();
            }
        }
    }

    public virtual void StartDialogue()
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
                    dialogueManager.StartDialogue(currentDialogue.lines, currentDialogue.choices);
                    dialogueManager.dialogueUI.HidePressXMessage();       // Usa o DialogueUI para esconder a mensagem "Press X"
                    canShowDialogue = false;
                    break;
                }
                else
                {
                    dialogueManager.dialogueUI.HidePressXMessage();
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
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
                    dialogueManager.dialogueUI.ShowPressXMessage(); // Usa o DialogueUI para mostrar a mensagem "Press X"
                    return;
                }
            }
            dialogueManager.dialogueUI.HidePressXMessage();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerCollider == other)
            {
                playerCollider = null;
                canShowDialogue = false;
                dialogueManager.dialogueUI.HidePressXMessage();
            }
        }
    }

    public void LoadModifiableDialogue()
    {
        if (File.Exists(modifiableDialoguePath))
        {
            string jsonContent = File.ReadAllText(modifiableDialoguePath);
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(jsonContent);
        }
    }

    public void SaveModifiableDialogue()
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
