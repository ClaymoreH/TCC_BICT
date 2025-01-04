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
    public int id;          // ID do di�logo
    public int status;      // Status do di�logo
    public int next_status; // Pr�ximo status do di�logo
    public string[] lines;  // Linhas do di�logo
    public string[] choices; // Op��es de escolha
    public int[] nextDialogues; // �ndices dos di�logos seguintes para cada escolha
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
        InitializeDialogue(); // Chama o m�todo para iniciar o carregamento do di�logo
    }

    private void InitializeDialogue()
    {
        LoadDialogueData();  // Chama o m�todo para carregar o arquivo JSON
        DisplayDialogue(currentDialogueIndex);  // Exibe o di�logo inicial
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
            Debug.Log("Conte�do do JSON: " + jsonContent);  // Verifique se o conte�do foi lido corretamente

            // Deserializa o JSON para o formato de ChoiceDialogueDatabase
            ChoiceDialogueDatabase container = JsonUtility.FromJson<ChoiceDialogueDatabase>(jsonContent);

            if (container != null && container.dialogues != null && container.dialogues.Length > 0)
            {
                dialogues = container.dialogues; // Carrega os di�logos do JSON
                Debug.Log("Di�logos carregados com sucesso.");
            }
            else
            {
                Debug.LogError("O arquivo JSON foi lido, mas n�o cont�m di�logos v�lidos.");
            }
        }
        else
        {
            Debug.LogError($"Arquivo JSON n�o encontrado em {filePath}. Verifique o caminho e o nome do arquivo.");
        }
    }

    public void DisplayDialogue(int dialogueIndex)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogError("Nenhum di�logo encontrado no banco de dados.");
            return;
        }

        if (dialogueIndex < 0 || dialogueIndex >= dialogues.Length)
        {
            Debug.LogError("�ndice de di�logo inv�lido.");
            return;
        }

        ChoiceDialogueData dialogue = dialogues[dialogueIndex];
        
        if (dialogue == null)
        {
            Debug.LogError("Di�logo n�o encontrado.");
            return;
        }

        // Verifique se questionText n�o � nulo
        if (questionText == null)
        {
            Debug.LogError("questionText n�o est� atribu�do.");
            return;
        }

        if (dialogue.lines != null)
        {
            questionText.text = string.Join("\n", dialogue.lines); // Exibe todas as linhas do di�logo
        }
        else
        {
            Debug.LogError("As linhas do di�logo s�o nulas.");
        }

        // Verifique se os bot�es est�o atribu�dos
        if (choiceButtons == null || choiceButtons.Length == 0)
        {
            Debug.LogError("Nenhum bot�o de escolha foi atribu�do.");
            return;
        }

        // Preenche os bot�es de escolha com base nas op��es do di�logo
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            // Verificar se o bot�o n�o est� nulo
            if (choiceButtons[i] == null)
            {
                Debug.LogError($"Bot�o de escolha {i} est� nulo.");
                continue;
            }

            if (i < dialogue.choices.Length)
            {
                TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText == null)
                {
                    Debug.LogError($"Bot�o de escolha {i} n�o cont�m o componente TMP_Text.");
                    continue;
                }

                buttonText.text = dialogue.choices[i]; // Define o texto do bot�o para cada escolha
                int choiceIndex = i; // Captura o �ndice atual
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceMade(choiceIndex)); // Define o comportamento ao clicar no bot�o
                choiceButtons[i].gameObject.SetActive(true); // Torna o bot�o vis�vel
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false); // Torna o bot�o invis�vel se n�o houver mais op��es
            }
        }
    }

    private void OnChoiceMade(int choiceIndex)
    {
        // L�gica para lidar com a escolha do jogador
        if (choiceIndex < dialogues[currentDialogueIndex].nextDialogues.Length)
        {
            currentDialogueIndex = dialogues[currentDialogueIndex].nextDialogues[choiceIndex];
            DisplayDialogue(currentDialogueIndex);
        }
        else
        {
            Debug.LogError("�ndice de escolha inv�lido.");
        }
    }
}
