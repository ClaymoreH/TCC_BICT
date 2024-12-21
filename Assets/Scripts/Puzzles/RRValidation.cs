using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importação necessária para TextMeshPro
using System.Linq;

public class Validator : MonoBehaviour
{
    public static List<string> ValidateProcessing(List<ProcessData> processes, int Quantum)
    {
        var errors = new List<string>();
        
        // Agrupa os objetos por processoID
        var groupedProcesses = processes.GroupBy(p => p.processoID)
                                        .OrderBy(g => g.Key);

        foreach (var processGroup in groupedProcesses)
        {
            int previousTableID = -1;
            int totalExecutedTime = 0;
            int valorOriginal = processGroup.First().ValorOriginal;
            bool appearedInFirstCycle = false;

            foreach (var process in processGroup.OrderBy(p => p.tableID))
            {
                // Validação: Ordem crescente dos ciclos
                if (process.tableID <= previousTableID)
                {
                    errors.Add($"Erro: Processo {process.processoID} está fora de ordem nos ciclos.");
                }

                // Validação: Aparição no ciclo inicial
                if (process.tableID == 1)
                {
                    appearedInFirstCycle = true;
                }
                
                // Soma tempo executado
                totalExecutedTime += process.tempoExecucao;
                
                previousTableID = process.tableID;
            }
            
            // Verificação se apareceu no ciclo inicial
            if (!appearedInFirstCycle)
            {
                errors.Add($"Erro: Processo {processGroup.Key} não apareceu no ciclo inicial.");
            }

            // Validação: tempoExecucaoTotal = ValorOriginal
            if (totalExecutedTime != valorOriginal)
            {
                errors.Add($"Erro: Processo {processGroup.Key} - Tempo Executado ({totalExecutedTime}) não corresponde ao ValorOriginal ({valorOriginal}).");
            }
        }

        // Resultado
        return errors.Count > 0 ? errors : new List<string> { "Validação bem-sucedida. Nenhum erro encontrado." };
    }
}

// Classe auxiliar para armazenar dados dos processos
public class ProcessData
{
    public int tableID;         // Identificador do ciclo
    public int processoID;      // Identificador do objeto
    public int tempoExecucao;   // Tempo executado no ciclo
    public int ValorOriginal;   // Tempo total necessário para o objeto
}
