using UnityEngine;
using System.IO;

[System.Serializable]
public class ChoiceDialogueDatabase
{
    public ChoiceDialogueData[] dialogues;
}

[System.Serializable]
public class ChoiceDialogueData
{
    public int id; // ID do di�logo
    public int status;
    public int next_status;
    public string[] lines;
    public string[] choices;
    public int[] nextDialogues;
    public string[] missionObjectives;
    public int locationId; // Adicionado: ID do local associado
}

public class ChoiceDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue UI")]
    public DialogueUI dialogueUI; // Refer�ncia ao script de UI

    [Header("JSON Data")]
    public string jsonFilePath = "Dialogues.json";
    private ChoiceDialogueData[] dialogues;
    private int currentDialogueIndex = 0;

    void Start()
    {
        if (LoadDialogueData())
        {
            DisplayDialogue(currentDialogueIndex);
        }
    }

    private bool LoadDialogueData()
    {
        string filePath = Path.Combine(Application.dataPath, "Scripts/Dialogue", jsonFilePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON file not found at {filePath}");
            return false;
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            var container = JsonUtility.FromJson<ChoiceDialogueDatabase>(jsonContent);

            if (container?.dialogues == null || container.dialogues.Length == 0)
            {
                Debug.LogError("No valid dialogues found in the JSON file.");
                return false;
            }

            dialogues = container.dialogues;
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error reading JSON file: {ex.Message}");
            return false;
        }
    }

    private void DisplayDialogue(int dialogueIndex)
    {
        if (dialogues == null || dialogueIndex < 0 || dialogueIndex >= dialogues.Length)
        {
            Debug.LogError("Invalid dialogue index.");
            return;
        }

        var dialogue = dialogues[dialogueIndex];

        // Usar o DialogueUI para exibir as linhas
        dialogueUI.StartDialogue(dialogue.lines);

        // Opcional: Atualize outras informa��es relacionadas, se necess�rio
        dialogueUI.ShowPressXMessage(); // Se necess�rio mostrar mensagem para avan�ar
    }

    void Update()
    {
        if (dialogueUI.IsDialogueActive) return;

        // Verificar se o jogador est� pronto para tomar uma decis�o
        if (dialogues[currentDialogueIndex].choices.Length > 0)
        {
            DisplayChoices();
        }
    }

    private void DisplayChoices()
    {
        var dialogue = dialogues[currentDialogueIndex];

        dialogueUI.HidePressXMessage(); // Ocultar mensagem "pressione X"
        dialogueUI.SetChoices(dialogue.choices, HandleChoice); // Configurar escolhas
    }

    private void HandleChoice(int choiceIndex)
    {
        var dialogue = dialogues[currentDialogueIndex];

        if (choiceIndex < dialogue.nextDialogues.Length)
        {
            currentDialogueIndex = dialogue.nextDialogues[choiceIndex];
            DisplayDialogue(currentDialogueIndex);
        }
        else
        {
            Debug.LogError("Invalid choice index.");
        }
    }
}
