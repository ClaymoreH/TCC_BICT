using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RRManager : PuzzleManager
{
    [Header("Configurações do Puzzle")]
    public Transform panelTransform;
    public Transform painelProcessos; // Novo painel que contém todos os processos

    public override void ValidarPuzzle()
    {
        ValidateRoundRobinCycles();
    }

    // Método para validar os ciclos de Round Robin
    private void ValidateRoundRobinCycles()
    {
        // Variável para controlar se ocorreu algum erro durante a validação
        bool erroEncontrado = false;

        // Encontrar todos os RRSlotManagers dentro do painel
        RRSlotManager[] slotManagersInPanel = panelTransform.GetComponentsInChildren<RRSlotManager>();

        // Dicionário para acompanhar a execução de cada processo por ciclo
        Dictionary<int, HashSet<int>> processCycles = new Dictionary<int, HashSet<int>>();

        // Dicionário para armazenar a última aparição do tempoExecucaoTotal de cada objeto
        Dictionary<GameObject, int> lastExecutionTime = new Dictionary<GameObject, int>();

        // Dicionário para somar o quantum subtraído por processo
        Dictionary<int, int> processQuantumSum = new Dictionary<int, int>();

        // Lista para armazenar objetos com erro de ciclo
        List<GameObject> erroDeCicloObjects = new List<GameObject>();

        foreach (var slotManager in slotManagersInPanel)
        {
            Debug.Log($"Validando RRSlotManager com ID: {slotManager.tableID}");

            // Iterar sobre cada objeto no SlotManager
            foreach (var entry in slotManager.quantumSubtracted)
            {
                GameObject obj = entry.Key; // Objeto processado
                int quantumUsed = entry.Value; // Quantum total usado no objeto

                // Acessar o componente PuzzleObjectData para pegar processo, tempoExecucaoTotal e ValorOriginal
                PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
                if (objectData != null)
                {
                    int processo = objectData.processo; // ID do processo
                    int valorOriginal = objectData.ValorOriginal;
                    int tempoExecucaoTotal = objectData.tempoExecucaoTotal;

                    // Verificar se o objeto foi processado no ciclo correto
                    if (!processCycles.ContainsKey(processo))
                    {
                        processCycles[processo] = new HashSet<int>();
                    }

                    // Verificar se o objeto já apareceu em ciclos anteriores
                    int maxCycleAppeared = processCycles[processo].Count > 0 ? processCycles[processo].Max() : 0;

                    // Se o objeto aparece no ciclo atual, deve ter aparecido em todos os ciclos anteriores
                    if (slotManager.tableID > maxCycleAppeared + 1)
                    {
                        // Erro de ciclo: O objeto pulou ciclos
                        Debug.LogError($"Erro de Aparição Irregular: Objeto {obj.name} pulou ciclos. " +
                            $"Apareceu no ciclo {slotManager.tableID}, mas o ciclo {maxCycleAppeared + 1} não foi processado.");
                        
                        // Marcar objeto como com erro de ciclo
                        erroDeCicloObjects.Add(obj);
                        ExibirFeedback($"Erro de ciclo no processo {objectData.processo}: ciclo inválido!", errorSound);
                        erroEncontrado = true; // Marcar que ocorreu erro
                        return; // Interrompe a validação após o erro
                    }

                    // Agora, adiciona o ciclo atual ao conjunto de ciclos em que o processo foi executado
                    processCycles[processo].Add(slotManager.tableID);

                    // Somar o quantum subtraído para o processo específico
                    if (!processQuantumSum.ContainsKey(processo))
                    {
                        processQuantumSum[processo] = 0;
                    }
                    processQuantumSum[processo] += quantumUsed;

                    // Verificar a consistência de tempoExecucaoTotal no último ciclo
                    lastExecutionTime[obj] = tempoExecucaoTotal;
                }
                else
                {
                    Debug.LogWarning($"Objeto {obj.name} não possui o componente PuzzleObjectData.");
                }
            }
        }

        // Validação final dos objetos, verificando se há algum erro antes
        foreach (var entry in lastExecutionTime)
        {
            GameObject obj = entry.Key;
            if (erroDeCicloObjects.Contains(obj))
            {
                // Se o objeto já tem erro de ciclo, pular validação
                continue;
            }

            int tempoExecucaoTotal = entry.Value;
            PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
            if (objectData != null)
            {
                int valorOriginal = objectData.ValorOriginal;
                if (tempoExecucaoTotal != valorOriginal)
                {
                    // Erro de tempoExecucaoTotal: Valor incorreto
                    Debug.LogError($"Inconsistência de tempoExecucaoTotal: Objeto {obj.name} tem tempoExecucaoTotal de {tempoExecucaoTotal} mas ValorOriginal é {valorOriginal}.");
                    ExibirFeedback($"Erro no processo {objectData.processo}: tempo total incorreto.", errorSound);
                    erroEncontrado = true; // Marcar que ocorreu erro
                    return; // Interrompe a validação após o erro
                }
                else
                {
                    Debug.Log($"Objeto {obj.name} validado corretamente com tempoExecucaoTotal = {tempoExecucaoTotal}.");
                    ExibirFeedback($"Processo {objectData.processo} validado com sucesso!", successSound);
                }
            }
        }

        // Validação das tarefas do painel adicional (todos os processos precisam ser validados corretamente)
        bool todosProcessosValidos = true;
        var processosNoPainel = painelProcessos.GetComponentsInChildren<PuzzleObjectData>();
        foreach (var processo in processosNoPainel)
        {
            // Verifique se o processo foi validado corretamente
            if (processo.tempoExecucaoTotal != processo.ValorOriginal)
            {
                todosProcessosValidos = false;
                break;
            }
        }

        // Se todos os processos do painel adicional foram validados corretamente, exibe um feedback geral
        if (!erroEncontrado && todosProcessosValidos)
        {
            ExibirFeedback("Todos os processos concluídos com sucesso!", successSound);
        }
        else if (!todosProcessosValidos)
        {
            ExibirFeedback("Erro: Alguns processos não foram validados corretamente no painel.", errorSound);
        }
    }
}
