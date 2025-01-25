using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SJFGenerator : MonoBehaviour
{
    [Header("Configurações de Alertas")]
    public SJFDatabase sjfDatabase; // Referência ao ScriptableObject contendo os alertas
    public Transform alertParent; // Pai onde os alertas serão instanciados na hierarquia
    public GameObject alertPrefab; // Prefab base para o alerta

    [Header("Slots de Alertas")]
    public int numberOfAlerts = 4; // Quantidade de alertas a serem sorteados

    private List<SJFData> availableAlerts; // Lista interna de alertas disponíveis

    private void Start()
    {
        // Copia a lista de alertas do ScriptableObject
        availableAlerts = new List<SJFData>(sjfDatabase.alerts);
        GenerateRandomAlerts();
    }

private void GenerateRandomAlerts()
{
    // Seleciona o número de tarefas a serem exibidas
    int tasksToGenerate = Mathf.Min(numberOfAlerts, sjfDatabase.alerts.Count);

    // Lista para armazenar as tarefas sorteadas
    List<SJFData> selectedTasks = new List<SJFData>();

    // Sorteia as tarefas sem repetição
    List<SJFData> availableTasks = new List<SJFData>(sjfDatabase.alerts);
    for (int i = 0; i < tasksToGenerate; i++)
    {
        int randomIndex = Random.Range(0, availableTasks.Count);
        selectedTasks.Add(availableTasks[randomIndex]);
        availableTasks.RemoveAt(randomIndex);
    }

    // Instancia o prefab base
    GameObject newAlert = Instantiate(alertPrefab, alertParent);

    // Configura as tarefas no prefab
    SetupAlert(newAlert, selectedTasks);
}
private void SetupAlert(GameObject alertObject, List<SJFData> selectedTasks)
{
    for (int i = 1; i <= 5; i++) // Loop para as 5 partes
    {
        Transform partTransform = alertObject.transform.Find($"Parte{i}");

        if (partTransform != null)
        {
            if (i <= selectedTasks.Count) // Se ainda houver tarefas para atribuir
            {
                SJFData taskData = selectedTasks[i - 1]; // Pega a tarefa correspondente

                // Configura o ícone do alerta
                Image iconImage = partTransform.GetComponentInChildren<Image>();
                if (iconImage != null && taskData.icon != null)
                {
                    iconImage.sprite = taskData.icon;
                }

                // Atribui os valores ao PuzzleObjectData no iconDrag
                GameObject iconDragObject = partTransform.Find("IconDrag").gameObject; // Assumindo que o objeto está nomeado como "IconDrag"
                if (iconDragObject != null)
                {
                    PuzzleObjectData puzzleObjectData = iconDragObject.GetComponent<PuzzleObjectData>();
                    if (puzzleObjectData != null)
                    {
                        puzzleObjectData.tempoExecucao = taskData.tempoExecucao;
                        puzzleObjectData.ordemChegada = taskData.ordemChegada;

                        Debug.Log($"Valores atribuídos ao PuzzleObjectData: TempoExecução = {taskData.tempoExecucao}, OrdemChegada = {taskData.ordemChegada}");
                    }
                    else
                    {
                        Debug.LogError("PuzzleObjectData não encontrado no IconDrag!");
                    }
                }
                else
                {
                    Debug.LogError("IconDrag não encontrado na Parte!");
                }

                // Configura os botões
                Button[] buttons = partTransform.GetComponentsInChildren<Button>();
                if (buttons.Length > 0)
                {
                    MultiTextDisplayManager displayManager = FindObjectOfType<MultiTextDisplayManager>();
                    if (displayManager != null)
                    {
                        TextContent buttonContent = new TextContent
                        {
                            uiTexts = new List<string>
                            {
                                taskData.titulotext,
                                taskData.tempotext,
                                taskData.chegadatext,
                                taskData.descricaotext
                            },
                            tmpTexts = new List<string>
                            {
                                taskData.titulotext,
                                taskData.tempotext,
                                taskData.chegadatext,
                                taskData.descricaotext
                            },
                            associatedButtonIndices = new List<int> { displayManager.buttons.Count }
                        };

                        displayManager.buttonContents.Add(buttonContent);

                        foreach (Button button in buttons)
                        {
                            displayManager.buttons.Add(button);
                            button.onClick.AddListener(() => displayManager.HandleButtonClick(buttonContent.associatedButtonIndices[0]));
                        }
                    }
                    else
                    {
                        Debug.LogError("MultiTextDisplayManager não encontrado na cena!");
                    }
                }
                else
                {
                    Debug.LogError($"Nenhum botão encontrado na Parte{i} do prefab!");
                }
            }
            else
            {
                partTransform.gameObject.SetActive(false); // Desativa a parte se não houver mais tarefas
            }
        }
        else
        {
            Debug.LogError($"Parte{i} não encontrada no prefab do alerta!");
        }
    }
}


}
