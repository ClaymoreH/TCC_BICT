using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RRGenerator : MonoBehaviour
{
    [Header("Configurações de Alertas")]
    public RRDatabase rrDatabase; // Referência ao ScriptableObject contendo os alertas
    public Transform alertParent; // Pai onde os alertas serão instanciados na hierarquia
    public GameObject alertPrefab; // Prefab base para o alerta

    [Header("Slots de Alertas")]
    public int numberOfAlerts = 4; // Quantidade de alertas a serem sorteados
    public int minExecutionTime = 1; // Tempo de execução mínimo
    public int maxExecutionTime = 10; // Tempo de execução máximo

    private List<RRData> availableAlerts; // Lista interna de alertas disponíveis

    private void Start()
    {
        // Copia a lista de alertas do ScriptableObject
        availableAlerts = new List<RRData>(rrDatabase.alerts);
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
    int tasksToGenerate = Mathf.Min(numberOfAlerts, rrDatabase.alerts.Count);

    // Gera tempos de execução aleatórios
    List<int> executionTimes = new List<int>();
    int totalExecutionTime = 0;

    for (int i = 0; i < tasksToGenerate; i++)
    {
        // Define um tempo de execução aleatório
        int executionTime = Random.Range(minExecutionTime, maxExecutionTime + 1);
        executionTimes.Add(executionTime);
        totalExecutionTime += executionTime;
    }

    // Ajuste se o total de execução ultrapassar 25
    if (totalExecutionTime > 25)
    {
        float scaleFactor = 25f / totalExecutionTime; // Fator de escala para reduzir os tempos de execução
        for (int i = 0; i < executionTimes.Count; i++)
        {
            executionTimes[i] = Mathf.RoundToInt(executionTimes[i] * scaleFactor);
        }

        // Recalcula o total para garantir que não ultrapasse 25 (ajustando pequenos arredondamentos)
        totalExecutionTime = 0;
        foreach (var time in executionTimes)
        {
            totalExecutionTime += time;
        }

        // Caso o total ainda ultrapasse 25 devido a arredondamentos, ajuste manual
        if (totalExecutionTime > 25)
        {
            executionTimes[0] -= (totalExecutionTime - 25); // Ajuste o primeiro processo para garantir que o total seja 25
        }
    }

    // Seleciona tarefas do banco de dados
    List<RRData> availableTasks = new List<RRData>(rrDatabase.alerts);
    List<RRData> selectedTasks = new List<RRData>();

    for (int i = 0; i < tasksToGenerate; i++)
    {
        int randomIndex = Random.Range(0, availableTasks.Count);
        RRData taskData = availableTasks[randomIndex];

        // Configura os valores gerados
        taskData.tempoExecucao = executionTimes[i];
        taskData.ValorOriginal = executionTimes[i]; // Atribui também ao ValorOriginal
        Debug.Log($"Atribuído ValorOriginal: {taskData.ValorOriginal} para o processo {taskData.processo}");

        taskData.processo = i + 1; // Processo começa em 1

        // Configura o texto "Módulo A, B, C, D, E"
        string[] modules = { "A", "B", "C", "D", "E" };
        if (taskData.processo <= modules.Length)
        {
            taskData.titulotext = $"Módulo {modules[taskData.processo - 1]}";
        }
        else
        {
            taskData.titulotext = $"Módulo {taskData.processo}"; // Fallback para processos acima de 5
        }

        // Formata o texto do tempo
        taskData.tempotext = $"Tempo de Execução - {taskData.tempoExecucao}ms";

        selectedTasks.Add(taskData);
        availableTasks.RemoveAt(randomIndex);
    }

    // Embaralha as tarefas para distribuição aleatória
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

    private void SetupAlert(GameObject alertObject, List<RRData> selectedTasks)
    {
        string[] cores = { "Verde", "Amarelo", "Azul", "Rosa", "Vermelho" };

        for (int i = 0; i < cores.Length; i++) // Loop para as cores
        {
            Transform colorTransform = alertObject.transform.Find(cores[i]);

            if (colorTransform != null)
            {
                if (i < selectedTasks.Count) // Se ainda houver tarefas para atribuir
                {
                    RRData taskData = selectedTasks[i];

                    // Configura o ícone do alerta
                    Image iconImage = colorTransform.GetComponentInChildren<Image>();
                    if (iconImage != null && taskData.iconDrag != null)
                    {
                        iconImage.sprite = taskData.iconDrag;
                    }

                    // Atribui os valores ao PuzzleObjectData no iconDrag
                    GameObject iconDragObject = colorTransform.Find("IconDrag").gameObject;
                    if (iconDragObject != null)
                    {
                        PuzzleObjectData puzzleObjectData = iconDragObject.GetComponent<PuzzleObjectData>();
                        if (puzzleObjectData != null)
                        {
                            puzzleObjectData.tempoExecucao = taskData.tempoExecucao;
                            puzzleObjectData.processo = taskData.processo;
                        }
                    }

                    // Configura os botões
                    Button[] buttons = colorTransform.GetComponentsInChildren<Button>();
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
                    colorTransform.gameObject.SetActive(false); // Desativa a cor se não houver mais tarefas
                }
            }
        }
    }
    public void ClearAlerts()
    {
        // Remove todos os filhos do alertParent
        foreach (Transform child in alertParent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Todos os alertas foram removidos.");
    }
} 