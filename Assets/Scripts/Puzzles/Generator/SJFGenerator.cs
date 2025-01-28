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
    public int minExecutionTime = 1; // Tempo de execução mínimo
    public int maxExecutionTime = 10; // Tempo de execução máximo

    private List<SJFData> availableAlerts; // Lista interna de alertas disponíveis

    private void Start()
    {
        // Copia a lista de alertas do ScriptableObject
        availableAlerts = new List<SJFData>(sjfDatabase.alerts);
        GenerateRandomAlerts();
    }

public void GenerateRandomAlerts()
{
    // Limpa os alertas existentes no alertParent
    foreach (Transform child in alertParent)
    {
        Destroy(child.gameObject);
    }

    // Seleciona o número de tarefas a serem exibidas
    int tasksToGenerate = Mathf.Min(numberOfAlerts, sjfDatabase.alerts.Count);

    // Gera tempos de execução que somam exatamente 15ms
    List<int> executionTimes = new List<int>();
    int remainingExecutionTime = 15;

    for (int i = 0; i < tasksToGenerate; i++)
    {
        if (i == tasksToGenerate - 1)
        {
            // Último processo recebe o restante
            executionTimes.Add(remainingExecutionTime);
        }
        else
        {
            // Define um tempo de execução aleatório (1 a 5ms)
            int maxTime = Mathf.Min(5, remainingExecutionTime - (tasksToGenerate - i - 1));
            int executionTime = Random.Range(1, maxTime + 1);
            executionTimes.Add(executionTime);
            remainingExecutionTime -= executionTime;
        }
    }

    // Gera tempos de chegada variados
    List<int> arrivalTimes = new List<int> { 0 }; // Primeiro processo sempre chega no tempo 0

    for (int i = 1; i < tasksToGenerate; i++)
    {
        // Gera o tempo de chegada baseado no término de execução do processo anterior
        int earliestArrival = arrivalTimes[i - 1] + Random.Range(1, 3); // Pequena lacuna (1 a 2ms)
        int latestArrival = earliestArrival + Random.Range(0, 3); // Adiciona variação (0 a 2ms)
        int arrivalTime = Random.Range(earliestArrival, latestArrival + 1);

        // Garante que o próximo processo esteja disponível antes que todos os anteriores terminem
        arrivalTime = Mathf.Min(arrivalTime, arrivalTimes[i - 1] + executionTimes[i - 1]);
        arrivalTimes.Add(arrivalTime);
    }

    // Seleciona tarefas do banco de dados
    List<SJFData> availableTasks = new List<SJFData>(sjfDatabase.alerts);
    List<SJFData> selectedTasks = new List<SJFData>();

    for (int i = 0; i < tasksToGenerate; i++)
    {
        int randomIndex = Random.Range(0, availableTasks.Count);
        SJFData taskData = availableTasks[randomIndex];

        // Configura os tempos gerados
        taskData.tempoExecucao = executionTimes[i];
        taskData.ordemChegada = arrivalTimes[i];

        // Formata os textos para exibição
        taskData.tempotext = $"Tempo de Execução - {taskData.tempoExecucao}ms";
        taskData.chegadatext = $"Chegada - {taskData.ordemChegada}ms";

        selectedTasks.Add(taskData);
        availableTasks.RemoveAt(randomIndex);
    }

    // Embaralha as partes para distribuição aleatória
    ShuffleList(selectedTasks);

    // Instancia o prefab base
    GameObject newAlert = Instantiate(alertPrefab, alertParent);

    // Configura as tarefas no prefab
    SetupAlert(newAlert, selectedTasks);
}

// Função para embaralhar a lista
private void ShuffleList<T>(List<T> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        int randomIndex = Random.Range(i, list.Count);
        T temp = list[i];
        list[i] = list[randomIndex];
        list[randomIndex] = temp;
    }
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
                GameObject iconDragObject = partTransform.Find("IconDrag").gameObject;
                if (iconDragObject != null)
                {
                    PuzzleObjectData puzzleObjectData = iconDragObject.GetComponent<PuzzleObjectData>();
                    if (puzzleObjectData != null)
                    {
                        puzzleObjectData.tempoExecucao = taskData.tempoExecucao;
                        puzzleObjectData.ordemChegada = taskData.ordemChegada;
                    }
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
                            uiTexts = new List<string> { taskData.titulotext, taskData.tempotext, taskData.chegadatext, taskData.descricaotext },
                            tmpTexts = new List<string> { taskData.titulotext, taskData.tempotext, taskData.chegadatext, taskData.descricaotext },
                            associatedButtonIndices = new List<int> { displayManager.buttons.Count }
                        };

                        displayManager.buttonContents.Add(buttonContent);

                        foreach (Button button in buttons)
                        {
                            displayManager.buttons.Add(button);
                            button.onClick.AddListener(() => displayManager.HandleButtonClick(buttonContent.associatedButtonIndices[0]));
                        }
                    }
                }
            }
            else
            {
                partTransform.gameObject.SetActive(false); // Desativa a parte se não houver mais tarefas
            }
        }
    }
}

    private void ClearAlerts()
    {
        // Remove todos os filhos do alertParent
        foreach (Transform child in alertParent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Todos os alertas foram removidos.");
    }
}
