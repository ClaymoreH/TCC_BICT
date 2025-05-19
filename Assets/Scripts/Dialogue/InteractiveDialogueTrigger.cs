using System.IO;
using UnityEngine;
using System.Collections;


public class InteractiveDialogueTrigger : MonoBehaviour
{
    public TextAsset initialDialogueFile;
    public string modifiableDialoguePath;
    public int dialogueID;

    public DialogueManager dialogueManager;
    public DialogueDatabase dialogueDatabase;
    public DialogueData currentDialogue;

    public bool canShowDialogue = false;
    public Collider2D playerCollider = null;
    public GameObject callingObject;  // Definindo a variável para armazenar o objeto que chama

    public void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        string directoryPath = Path.Combine(Application.dataPath, "Scripts", "Dialogue");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);  // Cria o diretório se não existir
        }

        modifiableDialoguePath = Path.Combine(Application.dataPath, "Scripts", "Dialogue", "modifiableDialogue.json");

        if (File.Exists(modifiableDialoguePath))
        {
            File.Delete(modifiableDialoguePath);
        }

        File.WriteAllText(modifiableDialoguePath, initialDialogueFile.text);

        LoadModifiableDialogue();
    }

    public void Update()
    {
        // Verifique se o player ainda está dentro da área e, se for, mostre a mensagem "Press X"
        if (canShowDialogue && Input.GetKeyDown(KeyCode.X) && playerCollider != null)
        {
            if (!dialogueManager.IsDialogueActive)
            {
                StartDialogue();
            }
        }

        // Se o GameObject for destruído e a interação com o diálogo ainda estiver ativa, esconda a mensagem "Press X"
        if (playerCollider == null && canShowDialogue)
        {
            dialogueManager.dialogueUI.HidePressXMessage();
            canShowDialogue = false;
        }
    }

public virtual void StartDialogue()
{
    // Armazenar o GameObject atual que iniciou o diálogo
    callingObject = this.gameObject; 

    LoadModifiableDialogue();

    foreach (var dialogue in dialogueDatabase.dialogues)
    {
        if (dialogue.id == dialogueID)
        {
            if (dialogue.status == 1)
            {
                dialogue.status = dialogue.next_status;

                SaveModifiableDialogue();

                currentDialogue = dialogue;

                // Passar o callingObject para o DialogueManager
                dialogueManager.StartDialogue(currentDialogue.lines, currentDialogue.choices.ToArray(), callingObject);


                dialogueManager.dialogueUI.HidePressXMessage();
                canShowDialogue = false;

                StartCoroutine(WaitForDialogueEnd());
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
            LoadModifiableDialogue();

            foreach (var dialogue in dialogueDatabase.dialogues)
            {
                if (dialogue.id == dialogueID && dialogue.status == 1)
                {
                    playerCollider = other;
                    canShowDialogue = true;
                    dialogueManager.dialogueUI.ShowPressXMessage();
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

    public void RestoreInitialState()
    {
        if (initialDialogueFile != null)
        {
            File.WriteAllText(modifiableDialoguePath, initialDialogueFile.text);
            LoadModifiableDialogue();
        }
    }
public void SetDialogueStatus(int dialogueID, int newStatus)
{
    // Carrega os dados do JSON
    LoadModifiableDialogue();

    // Procura o diálogo com o ID correspondente
    foreach (var dialogue in dialogueDatabase.dialogues)
    {
        if (dialogue.id == dialogueID)
        {
            // Atualiza o status do diálogo
            dialogue.status = newStatus;

            // Salva as alterações no JSON
            SaveModifiableDialogue();

            Debug.Log($"O status do diálogo com ID {dialogueID} foi alterado para {newStatus}.");
            return;
        }
    }

    Debug.LogWarning($"Nenhum diálogo com o ID {dialogueID} foi encontrado.");
}

    private IEnumerator WaitForDialogueEnd()
    {
        while (dialogueManager.IsDialogueActive)
        {
            yield return null;
        }

        canShowDialogue = true;
        dialogueManager.dialogueUI.ShowPressXMessage();
    }

    // Método chamado quando o GameObject é destruído
    private void OnDestroy()
    {
        // Verifique se a mensagem "Press X" ainda deve ser escondida
        if (canShowDialogue)
        {
            dialogueManager.dialogueUI.HidePressXMessage();
        }
    }
}
