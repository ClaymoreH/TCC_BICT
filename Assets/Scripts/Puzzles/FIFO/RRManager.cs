using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RRManager : PuzzleManager
{
    [Header("Configurações do Puzzle")]
    public Transform panelTransform;
    public Transform painelProcessos; 

    public override void ValidarPuzzle()
    {
        ValidateRoundRobinCycles();
    }
    private void ValidateRoundRobinCycles()
    {

        // Obtenha todos os objetos de cada slot
        RRSlotManager[] slotManagersInPanel = panelTransform.GetComponentsInChildren<RRSlotManager>();
        Debug.Log($"Encontrados {slotManagersInPanel.Length} slotManagers no painel.");

        Dictionary<int, HashSet<int>> processCycles = new Dictionary<int, HashSet<int>>();
        Dictionary<GameObject, int> lastExecutionTime = new Dictionary<GameObject, int>();
        Dictionary<int, int> processQuantumSum = new Dictionary<int, int>();
        List<GameObject> erroDeCicloObjects = new List<GameObject>();

// Dicionário para armazenar as aparições de cada processo (processo -> lista de dropzoneIDs)
Dictionary<int, HashSet<int>> processAppearances = new Dictionary<int, HashSet<int>>();

// Recalcular dinamicamente as aparições de cada processo
foreach (var slotManager in slotManagersInPanel)
{
    Debug.Log($"Validando RRSlotManager com ID: {slotManager.tableID}");

    // Iterar sobre todos os objetos atualmente presentes no slotManager
    foreach (Transform slot in slotManager.GetComponentsInChildren<Transform>())
    {
        if (slot.childCount > 0) // Verifica se há objetos dentro do slot
        {
            foreach (Transform objTransform in slot)
            {
                GameObject obj = objTransform.gameObject;

                // Acessar o componente PuzzleObjectData para pegar o ID do processo
                PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
                if (objectData != null)
                {
                    int processo = objectData.processo; // ID do processo
                    int dropzoneID = objectData.dropzoneID; // ID da DropZone atual do objeto

                    // Certificar-se de que o processo já existe no dicionário
                    if (!processAppearances.ContainsKey(processo))
                    {
                        processAppearances[processo] = new HashSet<int>();
                    }

                    // Adicionar o dropzoneID ao conjunto de aparições
                    processAppearances[processo].Add(dropzoneID);
                    Debug.Log($"Objeto {obj.name} (Processo {processo}) está atualmente na DropZoneID: {dropzoneID}");
                }
                else
                {
                    Debug.LogWarning($"Objeto {obj.name} não possui o componente PuzzleObjectData.");
                }
            }
        }
    }
}

// Após a coleta dinâmica, validar a sequência dos DropZoneIDs de cada processo
Debug.Log("Validando a sequência dos DropZoneIDs de cada processo...");

bool erroEncontrado = false;
foreach (var entry in processAppearances)
{
    int processo = entry.Key;
    var aparicoes = entry.Value.OrderBy(id => id).ToList(); // Ordenar os IDs para verificar a sequência
    Debug.Log($"Processo {processo}: DropZoneIDs -> {string.Join(", ", aparicoes)}");

    // Verificar se os IDs são sequenciais
    for (int i = 0; i < aparicoes.Count; i++)
    {
        if (aparicoes[i] != i)
        {
            // Encontrado um erro de sequência
            Debug.LogWarning($"Erro: Processo {processo} tem DropZoneIDs não sequenciais! " +
                             $"Esperado: {i}, Encontrado: {aparicoes[i]}.");
            ExibirFeedback($"Erro no processo {processo}: DropZoneIDs não sequenciais.", errorSound);
            erroEncontrado = true;
            break;
        }
    }

    if (erroEncontrado) continue;

    // Validar o tempoExecucaoTotal na última DropZone
    int ultimaDropZone = aparicoes.Max();
    Debug.Log($"Validando tempoExecucaoTotal para o processo {processo} na última DropZoneID: {ultimaDropZone}");

    foreach (var slotManager in slotManagersInPanel)
    {
        foreach (Transform slot in slotManager.GetComponentsInChildren<Transform>())
        {
            if (slot.childCount > 0)
            {
                foreach (Transform objTransform in slot)
                {
                    GameObject obj = objTransform.gameObject;

                    // Acessar o componente PuzzleObjectData para verificar o processo e o dropzone
                    PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
                    if (objectData != null && objectData.processo == processo && objectData.dropzoneID == ultimaDropZone)
                    {
                        int valorOriginal = objectData.ValorOriginal;
                        int tempoExecucaoTotal = objectData.tempoExecucaoTotal;

                        // Validar se o tempoExecucaoTotal é igual ao valorOriginal
                        if (tempoExecucaoTotal != valorOriginal)
                        {
                            Debug.LogWarning($"Erro: Processo {processo} na última DropZoneID {ultimaDropZone} tem tempoExecucaoTotal = {tempoExecucaoTotal}, mas ValorOriginal = {valorOriginal}.");
                            ExibirFeedback($"Erro no processo {processo}: tempo total incorreto na última DropZone!", errorSound);
                            erroEncontrado = true;
                        }
                        else
                        {
                            Debug.Log($"Validação bem-sucedida: Processo {processo} na última DropZoneID {ultimaDropZone} tem tempoExecucaoTotal = {tempoExecucaoTotal} e ValorOriginal = {valorOriginal}.");
                            ExibirFeedback($"Processo {processo} validado com sucesso na última DropZone!", successSound);
                        }
                        break;
                    }
                }
            }
        }
    }
}

// Feedback geral
if (!erroEncontrado)
{
    Debug.Log("Validação finalizada com sucesso! Todos os processos são válidos.");
    ExibirFeedback("Validação concluída com sucesso! Todos os processos são válidos.", successSound);
}


    }
}
