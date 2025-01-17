using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class RRManager : PuzzleManager
{
    [Header("Configurações do Puzzle")]
    public Transform panelTransform; // Painel que contém os slots
    public Transform painelProcessos; // Painel que contém todos os processos

    [Header("Referência ao Puzzle")]
    public Puzzle puzzle;

    public override void ValidarPuzzle()
    {
        ValidateRoundRobinCycles();
    }

    private void ValidateRoundRobinCycles()
    {
        // Obtenha todos os objetos de cada slot
        RRSlotManager[] slotManagersInPanel = panelTransform.GetComponentsInChildren<RRSlotManager>();
        Debug.Log($"Encontrados {slotManagersInPanel.Length} slotManagers no painel.");

        // Obter todos os processos do painel
        List<GameObject> processosPainel = ObterProcessosDoPainel();
        Debug.Log($"Processos encontrados no painel de processos: {processosPainel.Count}");

        // Obter objetos presentes nos slots
        var objetosNosSlots = ObterObjetosNosSlots(slotManagersInPanel);

        // Dicionário para armazenar as aparições de cada processo (processo -> lista de dropzoneIDs)
        Dictionary<int, HashSet<int>> processAppearances = new Dictionary<int, HashSet<int>>();

        // Recalcular dinamicamente as aparições de cada processo
        foreach (var objectData in objetosNosSlots)
        {
            int processo = objectData.processo;
            int dropzoneID = objectData.dropzoneID;

            if (!processAppearances.ContainsKey(processo))
            {
                processAppearances[processo] = new HashSet<int>();
            }
            processAppearances[processo].Add(dropzoneID);
        }

        // Verificar se todos os processos foram alocados pelo menos uma vez
        if (!ValidarProcessosAlocados(processosPainel, objetosNosSlots))
        {
            return; // Encerrar se houver erro
        }

        Debug.Log("Validando a sequência dos DropZoneIDs de cada processo...");

        int errosEncontrados = 0;

        foreach (var entry in processAppearances)
        {
            int processo = entry.Key;
            var aparicoes = entry.Value.OrderBy(id => id).ToList();

            // Validar a sequência dos DropZoneIDs
            if (!ValidarSequenciaDropZones(processo, aparicoes))
            {
                errosEncontrados++;
                continue;
            }

            // Validar o tempoExecucaoTotal na última DropZone
            int ultimaDropZone = aparicoes.Max();
            ValidarTempoExecucao(objetosNosSlots, processo, ultimaDropZone, ref errosEncontrados);
        }

        // Feedback geral
        if (errosEncontrados == 0)
        {
            Debug.Log("Validação finalizada com sucesso! Todos os processos são válidos.");
            ExibirFeedbackUnificado("Validação concluída com sucesso! Todos os processos são válidos.", true);
            
            puzzle.CompletePuzzle();

            // Destruir o puzzle após um atraso de 2 segundos
            StartCoroutine(DestroyPuzzleWithDelay(2f));
        }
    }

    private List<GameObject> ObterProcessosDoPainel()
    {
        // Obter todos os processos no painel de processos
        List<GameObject> processos = new List<GameObject>();
        foreach (Transform child in painelProcessos)
        {
            processos.Add(child.gameObject);
        }
        return processos;
    }

    private List<PuzzleObjectData> ObterObjetosNosSlots(RRSlotManager[] slotManagers)
    {
        // Obter todos os objetos nos slots
        List<PuzzleObjectData> objetos = new List<PuzzleObjectData>();
        foreach (var slotManager in slotManagers)
        {
            foreach (Transform slot in slotManager.GetComponentsInChildren<Transform>())
            {
                if (slot.childCount > 0)
                {
                    foreach (Transform objTransform in slot)
                    {
                        var objectData = objTransform.GetComponent<PuzzleObjectData>();
                        if (objectData != null)
                        {
                            objetos.Add(objectData);
                        }
                        else
                        {
                            Debug.LogWarning($"Objeto {objTransform.gameObject.name} não possui o componente PuzzleObjectData.");
                        }
                    }
                }
            }
        }
        return objetos;
    }

    private bool ValidarProcessosAlocados(List<GameObject> processosPainel, List<PuzzleObjectData> objetosNosSlots)
    {
        // Obter IDs dos processos alocados
        HashSet<int> processosAlocados = objetosNosSlots
            .Select(obj => obj.processo)
            .ToHashSet();

        // Verificar se todos os processos do painel estão alocados
        bool todosProcessosAlocados = true;
        foreach (var processo in processosPainel)
        {
            var objectData = processo.GetComponent<PuzzleObjectData>();
            if (objectData != null && !processosAlocados.Contains(objectData.processo))
            {
                Debug.LogWarning($"Erro: O processo {objectData.processo} não foi alocado em nenhum slot.");
                ExibirFeedbackUnificado($"Erro: O processo {objectData.processo} não foi alocado!", false);
                todosProcessosAlocados = false;
            }
        }

        return todosProcessosAlocados;
    }

    private bool ValidarSequenciaDropZones(int processo, List<int> aparicoes)
    {
        for (int i = 0; i < aparicoes.Count; i++)
        {
            if (aparicoes[i] != i)
            {
                Debug.LogWarning($"Erro: Processo {processo} tem DropZoneIDs não sequenciais! Esperado: {i}, Encontrado: {aparicoes[i]}.");
                ExibirFeedbackUnificado($"Erro no processo {processo}: DropZoneIDs não sequenciais.", false);
                return false;
            }
        }
        Debug.Log($"Processo {processo}: DropZoneIDs -> {string.Join(", ", aparicoes)} (Sequência válida)");
        return true;
    }

    private void ValidarTempoExecucao(List<PuzzleObjectData> objetosNosSlots, int processo, int ultimaDropZone, ref int errosEncontrados)
    {
        foreach (var objectData in objetosNosSlots)
        {
            if (objectData.processo == processo && objectData.dropzoneID == ultimaDropZone)
            {
                if (objectData.tempoExecucaoTotal != objectData.ValorOriginal)
                {
                    Debug.LogWarning($"Erro: Processo {processo} na última DropZoneID {ultimaDropZone} tem tempoExecucaoTotal = {objectData.tempoExecucaoTotal}, mas ValorOriginal = {objectData.ValorOriginal}.");
                    ExibirFeedbackUnificado($"Erro no processo {processo}: tempo total incorreto na última DropZone!", false);
                    errosEncontrados++;
                }
                else
                {
                    Debug.Log($"Validação bem-sucedida: Processo {processo} validado na última DropZoneID {ultimaDropZone}.");
                    ExibirFeedbackUnificado($"Processo {processo} validado com sucesso na última DropZone!", true);
                }
                break;
            }
        }
    }

    private void ExibirFeedbackUnificado(string mensagem, bool sucesso)
    {
        AudioClip som = sucesso ? successSound : errorSound;
        ExibirFeedback(mensagem, som);
    }

    private IEnumerator DestroyPuzzleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (puzzle != null)
        {
            Debug.Log("Destruindo o objeto puzzle após o atraso.");
            Destroy(puzzle.gameObject);
        }
    }
}
