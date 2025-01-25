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
    public int numberOfAlerts = 5; // Quantidade de alertas (fixa, baseada nas cores)

    private List<RRData> availableAlerts; // Lista interna de alertas disponíveis

    private void Start()
    {
        // Copia a lista de alertas do ScriptableObject
        availableAlerts = new List<RRData>(rrDatabase.alerts);
        GenerateAlerts();
    }

    private void GenerateAlerts()
    {
        // Garante que há alertas suficientes no banco de dados
        int tasksToGenerate = Mathf.Min(numberOfAlerts, availableAlerts.Count);

        // Lista para armazenar as tarefas sorteadas
        List<RRData> selectedTasks = new List<RRData>();

        // Sorteia as tarefas sem repetição
        for (int i = 0; i < tasksToGenerate; i++)
        {
            int randomIndex = Random.Range(0, availableAlerts.Count);
            selectedTasks.Add(availableAlerts[randomIndex]);
            availableAlerts.RemoveAt(randomIndex);
        }

        // Instancia o prefab base
        GameObject newAlert = Instantiate(alertPrefab, alertParent);

        // Configura as tarefas no prefab
        SetupAlert(newAlert, selectedTasks);
    }

    private void SetupAlert(GameObject alertObject, List<RRData> selectedTasks)
    {
        // Configura as 5 partes (Verde, Amarelo, Rosa, Azul, Vermelho)
        string[] colors = { "Verde", "Amarelo", "Rosa", "Azul", "Vermelho" };

        for (int i = 0; i < colors.Length; i++)
        {
            Transform colorTransform = alertObject.transform.Find(colors[i]);

            if (colorTransform != null)
            {
                if (i < selectedTasks.Count) // Se houver tarefas disponíveis
                {
                    RRData taskData = selectedTasks[i]; // Pega a tarefa correspondente

                    // Configura o ícone do alerta
                    Image iconImage = colorTransform.GetComponentInChildren<Image>();
                    if (iconImage != null && taskData.iconDrag != null)
                    {
                        iconImage.sprite = taskData.iconDrag;
                    }

                    // Atribui os valores ao PuzzleObjectData no DragAndDrop
                    GameObject dragObject = colorTransform.Find("DragAndDrop").gameObject;
                    if (dragObject != null)
                    {
                        PuzzleObjectData puzzleObjectData = dragObject.GetComponent<PuzzleObjectData>();
                        if (puzzleObjectData != null)
                        {
                            puzzleObjectData.tempoExecucao = taskData.tempoExecucao;
                            puzzleObjectData.ValorOriginal = taskData.ValorOriginal;
                            puzzleObjectData.processo = taskData.processo;

                            Debug.Log($"Valores atribuídos ao PuzzleObjectData: TempoExecução = {taskData.tempoExecucao}, ValorOriginal = {taskData.ValorOriginal}, Processo = {taskData.processo}");
                        }
                        else
                        {
                            Debug.LogError("PuzzleObjectData não encontrado no DragAndDrop!");
                        }
                    }
                    else
                    {
                        Debug.LogError($"DragAndDrop não encontrado na cor {colors[i]}!");
                    }

                    // Configura os botões
                    Button button = colorTransform.GetComponentInChildren<Button>();
                    if (button != null)
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

                            displayManager.buttons.Add(button);
                            button.onClick.AddListener(() => displayManager.HandleButtonClick(buttonContent.associatedButtonIndices[0]));
                        }
                        else
                        {
                            Debug.LogError("MultiTextDisplayManager não encontrado na cena!");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Botão não encontrado na cor {colors[i]}!");
                    }
                }
                else
                {
                    colorTransform.gameObject.SetActive(false); // Desativa a cor se não houver mais tarefas
                }
            }
            else
            {
                Debug.LogError($"Transform não encontrado para a cor {colors[i]} no prefab!");
            }
        }
    }
}
