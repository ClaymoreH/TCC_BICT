using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

[System.Serializable]
public class ChoiceDialogueDatabase
{
    public ChoiceDialogueData[] dialogues;
}

[System.Serializable]
public class ChoiceDialogueData
{
    public int id;          // ID do diálogo
    public int status;      // Status do diálogo
    public int next_status; // Próximo status do diálogo
    public string[] lines;  // Linhas do diálogo
    public string[] choices; // Opções de escolha
    public int[] nextDialogues; // Índices dos diálogos seguintes para cada escolha
}

public class ChoiceDialogueTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public Button[] choiceButtons;

    [Header("JSON Data")]
    public string jsonFilePath = "Dialogues.json";  // Nome do arquivo JSON
    private ChoiceDialogueData[] dialogues;
    private int currentDialogueIndex = 0;

    void Start()
    {
        InitializeDialogue(); // Chama o método para iniciar o carregamento do diálogo
    }

    private void InitializeDialogue()
    {
        LoadDialogueData();  // Chama o método para carregar o arquivo JSON
        DisplayDialogue(currentDialogueIndex);  // Exibe o diálogo inicial
    }

    private void LoadDialogueData()
    {
        // Caminho para o arquivo JSON na pasta "Assets/Scripts/Dialogue"
        string filePath = Path.Combine(Application.dataPath, "Scripts/Dialogue", jsonFilePath);
        Debug.Log($"Caminho do arquivo JSON: {filePath}");

        if (File.Exists(filePath))
        {
            Debug.Log("Arquivo JSON encontrado.");
            string jsonContent = File.ReadAllText(filePath);
            Debug.Log("Conteúdo do JSON: " + jsonContent);  // Verifique se o conteúdo foi lido corretamente

            // Deserializa o JSON para o formato de ChoiceDialogueDatabase
            ChoiceDialogueDatabase container = JsonUtility.FromJson<ChoiceDialogueDatabase>(jsonContent);

            if (container != null && container.dialogues != null && container.dialogues.Length > 0)
            {
                dialogues = container.dialogues; // Carrega os diálogos do JSON
                Debug.Log("Diálogos carregados com sucesso.");
            }
            else
            {
                Debug.LogError("O arquivo JSON foi lido, mas não contém diálogos válidos.");
            }
        }
        else
        {
            Debug.LogError($"Arquivo JSON não encontrado em {filePath}. Verifique o caminho e o nome do arquivo.");
        }
    }

    public void DisplayDialogue(int dialogueIndex)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogError("Nenhum diálogo encontrado no banco de dados.");
            return;
        }

        if (dialogueIndex < 0 || dialogueIndex >= dialogues.Length)
        {
            Debug.LogError("Índice de diálogo inválido.");
            return;
        }

        ChoiceDialogueData dialogue = dialogues[dialogueIndex];
        
        if (dialogue == null)
        {
            Debug.LogError("Diálogo não encontrado.");
            return;
        }

        // Verifique se questionText não é nulo
        if (questionText == null)
        {
            Debug.LogError("questionText não está atribuído.");
            return;
        }

        if (dialogue.lines != null)
        {
            questionText.text = string.Join("\n", dialogue.lines); // Exibe todas as linhas do diálogo
        }
        else
        {
            Debug.LogError("As linhas do diálogo são nulas.");
        }

        // Verifique se os botões estão atribuídos
        if (choiceButtons == null || choiceButtons.Length == 0)
        {
            Debug.LogError("Nenhum botão de escolha foi atribuído.");
            return;
        }

        // Preenche os botões de escolha com base nas opções do diálogo
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            // Verificar se o botão não está nulo
            if (choiceButtons[i] == null)
            {
                Debug.LogError($"Botão de escolha {i} está nulo.");
                continue;
            }

            if (i < dialogue.choices.Length)
            {
                TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText == null)
                {
                    Debug.LogError($"Botão de escolha {i} não contém o componente TMP_Text.");
                    continue;
                }

                buttonText.text = dialogue.choices[i]; // Define o texto do botão para cada escolha
                int choiceIndex = i; // Captura o índice atual
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceMade(choiceIndex)); // Define o comportamento ao clicar no botão
                choiceButtons[i].gameObject.SetActive(true); // Torna o botão visível
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false); // Torna o botão invisível se não houver mais opções
            }
        }
    }

    private void OnChoiceMade(int choiceIndex)
    {
        // Lógica para lidar com a escolha do jogador
        if (choiceIndex < dialogues[currentDialogueIndex].nextDialogues.Length)
        {
            currentDialogueIndex = dialogues[currentDialogueIndex].nextDialogues[choiceIndex];
            DisplayDialogue(currentDialogueIndex);
        }
        else
        {
            Debug.LogError("Índice de escolha inválido.");
        }
    }
}
