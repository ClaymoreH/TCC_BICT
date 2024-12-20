using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public int id; // ID do di�logo
    public string[] lines; // Linhas do di�logo
    public int type; // Tipo do di�logo (0 = autom�tico, 1 = interativo)
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues; // Array de di�logos
}

public class DialogueTrigger : MonoBehaviour
{
    public TextAsset dialogueFile; // Arquivo JSON com os di�logos
    public int dialogueID; // ID do di�logo associado a este trigger

    private DialogueUI dialogueUI; // Refer�ncia ao sistema de UI de di�logo
    private DialogueDatabase dialogueDatabase; // Base de dados de di�logos
    private DialogueData currentDialogue; // Dados do di�logo atual
    private bool canShowDialogue = false; // Controle para permitir intera��es interativas
    private bool hasShownAutomaticDialogue = false; // Controle para di�logos autom�ticos serem exibidos uma vez
    private bool isAutomaticDialogue = false; // Controle para identificar se o di�logo atual � autom�tico

    void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();

        // Carrega o JSON contendo os di�logos
        if (dialogueFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(dialogueFile.text);
        }

        // Subscreve o evento de fim de di�logo
        dialogueUI.OnDialogueEnded += OnDialogueEnded;
    }

    void OnDestroy()
    {
        // Remove subscri��o para evitar erros ao destruir objetos
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
                    currentDialogue = dialogue;

                    if (dialogue.type == 0 && !hasShownAutomaticDialogue) // Di�logo autom�tico
                    {
                        dialogueUI.StartDialogue(dialogue.lines);
                        hasShownAutomaticDialogue = true; // Marca o di�logo como exibido
                        isAutomaticDialogue = true; // Marca que o di�logo atual � autom�tico
                    }
                    else if (dialogue.type == 1) // Di�logo interativo
                    {
                        canShowDialogue = true;
                        dialogueUI.ShowPressXMessage(); // Mostra "Pressione X para interagir"
                        isAutomaticDialogue = false; // Marca que o di�logo atual n�o � autom�tico
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
            dialogueUI.HidePressXMessage(); // Esconde "Pressione X" ao sair da �rea
        }
    }

    void Update()
    {
        // Para di�logos interativos, verifica se o jogador apertou X
        if (canShowDialogue && Input.GetKeyDown(KeyCode.X))
        {
            dialogueUI.StartDialogue(currentDialogue.lines);
            dialogueUI.HidePressXMessage(); // Esconde "Pressione X" ao iniciar o di�logo
            canShowDialogue = false; // Impede que o jogador interaja novamente enquanto o di�logo est� ativo
        }
    }

    private void OnDialogueEnded()
    {
        // Apenas para di�logos interativos (type == 1), reabilita a intera��o
        if (currentDialogue != null && currentDialogue.type == 1)
        {
            canShowDialogue = true; // Permite interagir novamente
            dialogueUI.ShowPressXMessage(); // Mostra "Pressione X" novamente
        }

        // Impede que mensagens de "Pressione X" sejam mostradas ap�s di�logos autom�ticos
        if (isAutomaticDialogue)
        {
            isAutomaticDialogue = false; // Reseta o controle para futuros di�logos
            dialogueUI.HidePressXMessage(); // Garante que "Pressione X" n�o apare�a
        }
    }
}