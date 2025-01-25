using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Necessário para manipular Image no Canvas

public class FIFOGenerator : MonoBehaviour
{
    [Header("Configurações de Alertas")]
    public FIFODatabase fifoDatabase;  // Referência ao ScriptableObject contendo os alertas
    public Transform alertParent;  // Pai onde os alertas serão instanciados na hierarquia
    public GameObject alertPrefab;  // Prefab base para o IconDrag que já contém DragAndDrop e PuzzleObjectData

    [Header("Slots de Alertas")]
    public int numberOfAlerts = 4;  // Quantidade de alertas a serem sorteados

    private List<FIFOData> availableAlerts;  // Lista interna de alertas disponíveis

    private void Start()
    {
        // Copia a lista de alertas do ScriptableObject para evitar modificações na original
        availableAlerts = new List<FIFOData>(fifoDatabase.alerts);
        GenerateRandomAlerts();
    }

    private void GenerateRandomAlerts()
    {
        // Garante que o número de alertas não excede a quantidade de dados disponíveis
        int alertsToGenerate = Mathf.Min(numberOfAlerts, availableAlerts.Count);

        for (int i = 0; i < alertsToGenerate; i++)
        {
            // Seleciona um alerta aleatório
            int randomIndex = Random.Range(0, availableAlerts.Count);
            FIFOData randomAlert = availableAlerts[randomIndex];

            // Remove da lista para evitar repetição
            availableAlerts.RemoveAt(randomIndex);

            // Instancia o prefab e aplica as informações do alerta
            GameObject newAlert = Instantiate(alertPrefab, alertParent);
            SetupAlert(newAlert, randomAlert);
        }
    }

    private void SetupAlert(GameObject alertObject, FIFOData alertData)
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
