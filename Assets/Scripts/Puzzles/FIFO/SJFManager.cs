using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SJFManager : MonoBehaviour
{
    [Header("Configurações do Puzzle")]
    public SlotManager slotManager;
    public Button confirmButton;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip successSound;
    public AudioClip errorSound;

    [Header("UI de Feedback")]
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 1f;

    private void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ValidarSJF);
        }

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }

public void ValidarSJF()
{
    // Verifica se todas as linhas têm pelo menos um objeto
    for (int row = 1; row <= slotManager.tableGenerator.rows; row++)
    {
        bool linhaTemObjeto = false;

        // Itera pelas colunas da linha atual
        for (int col = 1; col <= slotManager.tableGenerator.columns; col++)
        {
            GameObject slot = slotManager.FindSlot(row, col);
            if (slot != null && slot.transform.childCount > 0)
            {
                linhaTemObjeto = true; // Marca que a linha tem pelo menos um objeto
                break;
            }
        }

        // Se encontrar uma linha vazia, exibe um erro
        if (!linhaTemObjeto)
        {
            ExibirFeedback("ERRO: Todas as linhas da tabela devem ter pelo menos um objeto.", errorSound);
            return;
        }
    }

    // Caso todas as linhas estejam preenchidas, inicia a validação SJF
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

        // Validação de ordem (checando chegada e tempo de execução)
        if (!ValidarOrdemTabelaLogic(objetos))
        {
            ExibirFeedback("ERRO: A ordem dos processos na tabela está incorreta!", errorSound);
            yield break; // Interrompe a execução
        }

        ExibirFeedback("Sucesso! A ordem está correta.", successSound);
    }

    private bool ValidarOrdemTabelaLogic(List<PuzzleObjectData> objetos)
    {
        // Simulação simplificada da execução para validar a ordem
        List<PuzzleObjectData> copiaObjetos = new List<PuzzleObjectData>(objetos);
        float currentTime = 0f;

        while (copiaObjetos.Count > 0)
        {
            // Adiciona processos prontos para execução
            List<PuzzleObjectData> prontosParaExecutar = copiaObjetos.FindAll(o => o.ordemChegada <= currentTime);

            if (prontosParaExecutar.Count == 0)
            {
                // Nenhum processo está pronto, avança o tempo
                currentTime = copiaObjetos[0].ordemChegada;
                continue;
            }

            // Ordena por menor tempo de execução
            prontosParaExecutar.Sort((a, b) => a.tempoExecucao.CompareTo(b.tempoExecucao));

            // Compara o primeiro da lista ordenada com o primeiro da tabela
            if (prontosParaExecutar[0] != copiaObjetos[0])
            {
                // Ordem incorreta na tabela
                return false;
            }

            // Remove o processo da cópia e avança o tempo
            copiaObjetos.Remove(prontosParaExecutar[0]);
            currentTime += prontosParaExecutar[0].tempoExecucao;
        }

        return true; // Ordem correta
    }

    private void ExibirFeedback(string mensagem, AudioClip som)
    {
        if (feedbackText != null)
        {
            feedbackText.text = mensagem;
        }

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
        }

        if (audioSource != null && som != null)
        {
            audioSource.PlayOneShot(som);
        }

        StartCoroutine(EsconderFeedback());
    }

    private IEnumerator EsconderFeedback()
    {
        yield return new WaitForSeconds(feedbackDuration);

        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
}
