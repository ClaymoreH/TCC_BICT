using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SJFManager : PuzzleManager
{
    [Header("Configurações do Puzzle")]
    public SlotManager slotManager;

    [Header("Referência ao Puzzle")]
    public Puzzle puzzle;

    [Header("Modos de Jogo")]
    public SJFGenerator sjfGenerator;
    public bool isStoryMode = true; 

    [Header("Configurações de Tempo e Pontuação")]
    public Timer timer; // Referência ao script Timer
    public TextMeshProUGUI randomModeScoreText; // Texto para exibir a pontuação no modo aleatório
    private int randomModeScore = 0; // Contador de puzzles resolvidos no modo aleatório

    private void Start()
    {
        base.Start();
    }

    public override void ValidarPuzzle()
    {
        StartCoroutine(ValidarOrdemTabela());
    }

    private IEnumerator ValidarOrdemTabela()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        yield return new WaitForSeconds(clickSound.length);

        // Coleta os primeiros objetos de cada linha
        List<PuzzleObjectData> objetos = new List<PuzzleObjectData>();

        for (int row = 1; row <= slotManager.tableGenerator.rows; row++)
        {
            for (int col = 1; col <= slotManager.tableGenerator.columns; col++)
            {
                GameObject slot = slotManager.FindSlot(row, col);
                if (slot != null && slot.transform.childCount > 0)
                {
                    PuzzleObjectData objectData = slot.transform.GetChild(0).GetComponent<PuzzleObjectData>();
                    if (objectData != null)
                    {
                        objetos.Add(objectData);
                        break; // Apenas o primeiro objeto da linha
                    }
                }
            }
        }

        // Verificar se todas as linhas têm pelo menos um objeto
        for (int row = 1; row <= slotManager.tableGenerator.rows; row++)
        {
            bool linhaTemObjeto = false;

            for (int col = 1; col <= slotManager.tableGenerator.columns; col++)
            {
                GameObject slot = slotManager.FindSlot(row, col);
                if (slot != null && slot.transform.childCount > 0)
                {
                    linhaTemObjeto = true;
                    break;
                }
            }

            if (!linhaTemObjeto)
            {
                HandleError("ERRO: Todas as linhas da tabela devem ter pelo menos um objeto.");
                yield break;
            }
        }

        // Validação de ordem (checando chegada e tempo de execução)
        if (!ValidarOrdemTabelaLogic(objetos))
        {
            HandleError("ERRO: A ordem dos processos na tabela está incorreta!");
            yield break;
        }

        ExibirFeedback("Sucesso! A ordem está correta.", successSound);
        yield return new WaitForSeconds(2f); // Espera antes de limpar a tabela


        // Ação para os modos de jogo
        if (isStoryMode)
        {
            if (puzzle != null)
            {
                puzzle.CompletePuzzle();
            }
        }
        else
        {
            randomModeScore++;
            UpdateRandomModeScoreUI();

            slotManager.ResetTable(); // Limpa a tabela usando o método ResetTable
            sjfGenerator.GenerateRandomAlerts();
            Debug.Log("Modo Aleatório: Reiniciando o puzzle.");
        }
    }

    private bool ValidarOrdemTabelaLogic(List<PuzzleObjectData> objetos)
    {
        // Simulação da execução para validar a ordem dos processos
        List<PuzzleObjectData> copiaObjetos = new List<PuzzleObjectData>(objetos);
        float currentTime = 0f;

        // Loop para simular a execução dos processos
        while (copiaObjetos.Count > 0)
        {
            // Adiciona processos que estão prontos para execução
            List<PuzzleObjectData> prontosParaExecutar = copiaObjetos.FindAll(o => o.ordemChegada <= currentTime);

            if (prontosParaExecutar.Count == 0)
            {
                // Se nenhum processo está pronto, avança o tempo para o próximo processo
                currentTime = copiaObjetos[0].ordemChegada;
                continue;
            }

            // Ordena os processos prontos por menor tempo de execução
            prontosParaExecutar.Sort((a, b) => a.tempoExecucao.CompareTo(b.tempoExecucao));

            // Verifica se o primeiro processo da lista de prontos corresponde ao primeiro da tabela
            if (prontosParaExecutar[0] != copiaObjetos[0])
            {
                // Se a ordem estiver incorreta, retorna false
                return false;
            }

            // Remove o processo da cópia e avança o tempo
            copiaObjetos.Remove(prontosParaExecutar[0]);
            currentTime += prontosParaExecutar[0].tempoExecucao;
        }

        // Se todos os processos foram executados corretamente
        return true;
    }

    // Método para lidar com erros (reduz tempo no modo história ou exibe feedback no modo aleatório)
    private void HandleError(string errorMessage)
    {
        ExibirFeedback(errorMessage, errorSound);

        if (isStoryMode)
        {
            // Reduz 30s do tempo no modo história
            if (timer != null)
            {
                timer.ReduceTime(30); // Reduz 60 segundos
            }
        }
    }

    // Método para atualizar o texto de pontuação no modo aleatório
    private void UpdateRandomModeScoreUI()
    {
        if (randomModeScoreText != null)
        {
            randomModeScoreText.text = $"Score: {randomModeScore}";
        }
    }
}
