using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RRManager : MonoBehaviour
{
    // Método para validar os ciclos de Round Robin
    public void ValidateRoundRobinCycles(Transform panelTransform)
    {
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
                        Debug.LogError($"Erro de Aparição Irregular: Objeto {obj.name} pulou ciclos. " +
                            $"Apareceu no ciclo {slotManager.tableID}, mas o ciclo {maxCycleAppeared + 1} não foi processado.");

                        // Marcar objeto como com erro de ciclo
                        erroDeCicloObjects.Add(obj);
                    }

                    // Agora, adiciona o ciclo atual ao conjunto de ciclos em que o processo foi executado
                    processCycles[processo].Add(slotManager.tableID);

                    // Somar o quantum subtraído para o processo específico
                    if (!processQuantumSum.ContainsKey(processo))
                    {
                        processQuantumSum[processo] = new int();
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

        // Validação final dos objetos apenas se não houver erro de pular ciclos
        foreach (var entry in lastExecutionTime)
        {
            GameObject obj = entry.Key;
            if (erroDeCicloObjects.Contains(obj))
            {
                // Pular a validação para objetos que possuem erro de ciclo
                continue;
            }

            int tempoExecucaoTotal = entry.Value;
            PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
            if (objectData != null)
            {
                int valorOriginal = objectData.ValorOriginal;
                if (tempoExecucaoTotal != valorOriginal)
                {
                    Debug.LogError($"Inconsistência de tempoExecucaoTotal: Objeto {obj.name} tem tempoExecucaoTotal de {tempoExecucaoTotal} mas ValorOriginal é {valorOriginal}.");
                }
                else
                {
                    Debug.Log($"Objeto {obj.name} validado corretamente com tempoExecucaoTotal = {tempoExecucaoTotal}.");
                }
            }
        }
    }
}
