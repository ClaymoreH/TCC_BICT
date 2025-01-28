using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class FIFOGenerator : MonoBehaviour
{
    [Header("Configurações de Alertas")]
    public FIFODatabase fifoDatabase;  // Referência ao ScriptableObject contendo os alertas
    public Transform alertParent;  // Pai onde os alertas serão instanciados na hierarquia
    public GameObject alertPrefab;  // Prefab base para o IconDrag que já contém DragAndDrop e PuzzleObjectData

    [Header("Slots de Alertas")]
    public int numberOfAlerts = 4;  // Quantidade de alertas a serem sorteados

    private List<FIFOData> availableAlerts;  // Lista temporária para os alertas disponíveis

    private void Start()
    {
        // Gera os primeiros alertas ao iniciar o jogo
        GenerateRandomAlerts();
    }

    public void GenerateRandomAlerts()
    {
        // Limpa os alertas anteriores no alertParent
        foreach (Transform child in alertParent)
        {
            Destroy(child.gameObject);
        }

        // Cria uma nova lista temporária baseada nos alertas do banco de dados
        availableAlerts = new List<FIFOData>(fifoDatabase.alerts);

        // Garante que o número de alertas não exceda a quantidade disponível
        int alertsToGenerate = Mathf.Min(numberOfAlerts, availableAlerts.Count);

        for (int i = 0; i < alertsToGenerate; i++)
        {
            // Seleciona um alerta aleatório
            int randomIndex = Random.Range(0, availableAlerts.Count);
            FIFOData randomAlert = availableAlerts[randomIndex];

            // Gera uma data e hora aleatória
            string generatedDate = GenerateRandomDate();
            string generatedTime = GenerateRandomTime();

            // Atualiza os valores de data e hora no objeto FIFOData
            randomAlert.data = generatedDate;
            randomAlert.hora = generatedTime;
            randomAlert.datatext = $"Data: {generatedDate} {generatedTime}";

            // Instancia o prefab e aplica as informações do alerta
            GameObject newAlert = Instantiate(alertPrefab, alertParent);
            SetupAlert(newAlert, randomAlert);
        }
    }

    /// <summary>
    /// Gera uma data aleatória no ano de 2104.
    /// </summary>
    private string GenerateRandomDate()
    {
        int month = Random.Range(1, 13); // Mês aleatório (1 a 12)
        int day;

        // Define o número máximo de dias com base no mês
        if (month == 2) // Fevereiro
            day = Random.Range(1, 29); // 28 dias (2104 não é bissexto)
        else if (month == 4 || month == 6 || month == 9 || month == 11) // Meses com 30 dias
            day = Random.Range(1, 31);
        else // Meses com 31 dias
            day = Random.Range(1, 32);

        return $"{day:00}/{month:00}/2104"; // Formato: DD/MM/2104
    }

    /// <summary>
    /// Gera uma hora aleatória no formato HH:MM.
    /// </summary>
    private string GenerateRandomTime()
    {
        int hour = Random.Range(0, 24); // Horas (0 a 23)
        int minute = Random.Range(0, 60); // Minutos (0 a 59)

        return $"{hour:00}:{minute:00}"; // Formato: HH:MM
    }

    public void SetupAlert(GameObject alertObject, FIFOData alertData)
    {
        // Configura o ícone do alerta (estático)
        Image iconImage = alertObject.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = alertData.icon;
        }
        else
        {
            Debug.LogError("Icon image not found on alert prefab!");
        }

        // Configura o ícone arrastável do alerta
        Image iconDragImage = alertObject.transform.Find("IconDrag")?.GetComponent<Image>();
        if (iconDragImage != null)
        {
            iconDragImage.sprite = alertData.iconDrag;
        }
        else
        {
            Debug.LogError("IconDrag image not found on alert prefab!");
        }

        // Configura o nome do alerta (Texto)
        TMPro.TMP_Text nameText = alertObject.transform.Find("Nome")?.GetComponent<TMPro.TMP_Text>();
        if (nameText != null)
        {
            nameText.text = alertData.name;
        }
        else
        {
            Debug.LogError("Nome text not found on alert prefab!");
        }

        // Configura os dados do PuzzleObjectData
        PuzzleObjectData puzzleData = alertObject.transform.Find("IconDrag")?.GetComponent<PuzzleObjectData>();
        if (puzzleData != null)
        {
            puzzleData.name = alertData.name;
            puzzleData.data = alertData.data;
            puzzleData.hora = alertData.hora;
        }
        else
        {
            Debug.LogError("PuzzleObjectData not found on IconDrag!");
        }

        // Passa o botão para o MultiTextDisplayManager
        Button alertButton = alertObject.GetComponent<Button>();
        if (alertButton != null)
        {
            MultiTextDisplayManager displayManager = FindObjectOfType<MultiTextDisplayManager>();
            if (displayManager != null)
            {
                // Crie ou encontre o conteúdo do botão
                TextContent buttonContent = new TextContent();
                buttonContent.uiTexts = new List<string> { alertData.titulotext, alertData.datatext, alertData.corpotext, alertData.logtext };
                buttonContent.tmpTexts = new List<string> { alertData.titulotext, alertData.datatext, alertData.corpotext, alertData.logtext };

                // Adiciona o índice do botão à lista de índices associados
                int buttonIndex = displayManager.buttons.Count;
                buttonContent.associatedButtonIndices = new List<int> { buttonIndex };

                // Associa o conteúdo ao botão
                displayManager.buttonContents.Add(buttonContent);

                // Adiciona o botão à lista de botões
                displayManager.buttons.Add(alertButton);
                alertButton.onClick.AddListener(() => displayManager.HandleButtonClick(buttonIndex));
            }
            else
            {
                Debug.LogError("MultiTextDisplayManager not found in the scene!");
            }
        }
        else
        {
            Debug.LogError("Button component not found in the alert prefab!");
        }
    }
}

