using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public int id; // ID do di�logo
    public string[] lines; // Linhas do di�logo
    public int type; // Tipo do di�logo (0 = autom�tico, 1 = interativo)
    public int status; // Tipo do di�logo (0 = autom�tico, 1 = interativo)
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues; // Array de di�logos
}

public class DialogueTrigger : MonoBehaviour
{
    public TextAsset dialogueFile; // Arquivo JSON com os diálogos
    public int dialogueID; // ID do diálogo associado a este trigger

    private DialogueUI dialogueUI; // Referência ao sistema de UI de diálogo
    private DialogueDatabase dialogueDatabase; // Base de dados de diálogos
    private DialogueData currentDialogue; // Dados do diálogo atual
    private bool canShowDialogue = false; // Controle para permitir interações interativas
    private bool hasShownAutomaticDialogue = false; // Controle para diálogos automáticos serem exibidos uma vez
    private bool isAutomaticDialogue = false; // Controle para identificar se o diálogo atual é automático

    void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();

        // Carrega o JSON contendo os diálogos
        if (dialogueFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(dialogueFile.text);
        }

        // Subscreve o evento de fim de diálogo
        dialogueUI.OnDialogueEnded += OnDialogueEnded;
    }

    void OnDestroy()
    {
        // Remove subscrição para evitar erros ao destruir objetos
        dialogueUI.OnDialogueEnded -= OnDialogueEnded;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dialogueDatabase != null)
        {
            foreach (var dialogue in dialogueDatabase.dialogues)
            {
                if (dialogue.id == dialogueID)
                {
                    // Verifica o status do diálogo
                    if (dialogue.status == 0)
                    {
                        // Diálogo inativo, não pode ser disparado
                        return;
                    }

                    currentDialogue = dialogue;

                    if (dialogue.type == 0 && !hasShownAutomaticDialogue && dialogue.status == 1) // Diálogo automático ativo
                    {
                        dialogueUI.StartDialogue(dialogue.lines);
                        hasShownAutomaticDialogue = true; // Marca o diálogo como exibido
                        isAutomaticDialogue = true; // Marca que o diálogo atual é automático
                    }
                    else if (dialogue.type == 1 && dialogue.status == 1) // Diálogo interativo ativo
                    {
                        canShowDialogue = true;
                        dialogueUI.ShowPressXMessage(); // Mostra "Pressione X para interagir"
                        isAutomaticDialogue = false; // Marca que o diálogo atual não é automático
                    }

                    break;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canShowDialogue = false;
            dialogueUI.HidePressXMessage(); // Esconde "Pressione X" ao sair da área
        }
    }

    void Update()
    {
        // Recarrega os diálogos a cada quadro
        ReloadDialogueData();

        // Para diálogos interativos, verifica se o jogador apertou X
        if (canShowDialogue && Input.GetKeyDown(KeyCode.X))
        {
            dialogueUI.StartDialogue(currentDialogue.lines);
            dialogueUI.HidePressXMessage(); // Esconde "Pressione X" ao iniciar o diálogo
            canShowDialogue = false; // Impede que o jogador interaja novamente enquanto o diálogo está ativo
        }
    }

    void ReloadDialogueData()
    {
        if (dialogueFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(dialogueFile.text);
        }

    }


    private void OnDialogueEnded()
    {
        // Atualiza o status do diálogo para concluído (2)
        if (currentDialogue != null)
        {
            currentDialogue.status = 2; // Define como concluído
        }

        // Apenas para diálogos interativos (type == 1), reabilita a interação se necessário
        if (currentDialogue != null && currentDialogue.type == 1)
        {
            canShowDialogue = true; // Permite interagir novamente
            dialogueUI.ShowPressXMessage(); // Mostra "Pressione X" novamente
        }

        // Impede que mensagens de "Pressione X" sejam mostradas após diálogos automáticos
        if (isAutomaticDialogue)
        {
            isAutomaticDialogue = false; // Reseta o controle para futuros diálogos
            dialogueUI.HidePressXMessage(); // Garante que "Pressione X" não apareça
        }
    }
}
