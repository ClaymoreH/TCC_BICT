using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJFManager : PuzzleManager
{
    [Header("Configurações do Puzzle")]
    public SlotManager slotManager;

    [Header("Referência ao Puzzle")]
    public Puzzle puzzle;

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
                ExibirFeedback("ERRO: Todas as linhas da tabela devem ter pelo menos um objeto.", errorSound);
                yield break;
            }
        }

        // Validação de ordem (checando chegada e tempo de execução)
        if (!ValidarOrdemTabelaLogic(objetos))
        {
            ExibirFeedback("ERRO: A ordem dos processos na tabela está incorreta!", errorSound);
            yield break;
        }

        ExibirFeedback("Sucesso! A ordem está correta.", successSound);
        // Destroi todos os objetos filhos do painel
        Transform panelTransform = puzzle.transform; // Substitua `puzzle` pelo seu painel, se necessário
        foreach (Transform child in panelTransform)
        {
            Destroy(child.gameObject);
        }

        if (puzzle != null)
        {
            puzzle.CompletePuzzle();
        }
            // Completar o puzzle
            if (puzzle != null)
            {
                puzzle.CompletePuzzle();
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
}
