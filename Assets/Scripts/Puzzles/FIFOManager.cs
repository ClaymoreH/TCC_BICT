using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FifoManager : PuzzleManager
{
    [Header("Gerador de Puzzles (Modo Aleatório)")]
    public FIFOGenerator fifoGenerator;
    public bool isStoryMode = true; 

    [Header("Configurações do Puzzle")]
    public List<Transform> slots;

    [Header("Referência ao Puzzle")]
    public Puzzle puzzle;

    [Header("Animação do Jogador")]
    public Animator playerAnimator;

    [Header("Configurações de Tempo e Pontuação")]
    public Timer timer; // Referência ao script Timer
    public TextMeshProUGUI randomModeScoreText; // Texto para exibir a pontuação no modo aleatório
    private int randomModeScore = 0; // Contador de puzzles resolvidos no modo aleatório

    private void Start()
    {
        base.Start();
        UpdateRandomModeScoreUI(); // Atualiza a pontuação inicial na UI
    }

    public override void ValidarPuzzle()
    {
        StartCoroutine(ValidateFIFO());
    }

    private IEnumerator ValidateFIFO()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        yield return new WaitForSeconds(clickSound.length);

        List<PuzzleObjectData> objectsInSlots = new List<PuzzleObjectData>();

        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                PuzzleObjectData objectData = slot.GetChild(0).GetComponent<PuzzleObjectData>();
                if (objectData != null)
                {
                    objectsInSlots.Add(objectData);
                }
                else
                {
                    HandleError("ERRO: Objeto inválido.");
                    yield break;
                }
            }
            else
            {
                HandleError("ERRO: Slot vazio.");
                yield break;
            }
        }

        for (int i = 0; i < objectsInSlots.Count - 1; i++)
        {
            try
            {
                System.DateTime currentDateTime = System.DateTime.Parse($"{objectsInSlots[i].data} {objectsInSlots[i].hora}");
                System.DateTime nextDateTime = System.DateTime.Parse($"{objectsInSlots[i + 1].data} {objectsInSlots[i + 1].hora}");

                if (currentDateTime > nextDateTime)
                {
                    HandleError("ERRO: A ordem dos processos está incorreta.");
                    yield break;
                }
            }
            catch (System.FormatException)
            {
                HandleError("ERRO: Formato inválido.");
                yield break;
            }
        }

        ExibirFeedback("Sucesso! A ordem está correta.", successSound);

        yield return new WaitForSeconds(2f); // Tempo para o feedback visual e sonoro

        if (isStoryMode)
        {
            // No modo história, fecha o painel e completa o puzzle
            Transform panelTransform = puzzle.transform;
            foreach (Transform child in panelTransform)
            {
                Destroy(child.gameObject);
            }

            if (puzzle != null)
            {
                puzzle.CompletePuzzle();
                PlayHappyAnimation();
            }
        }
        else
        {
            // No modo aleatório, aumenta a pontuação e exibe na UI
            randomModeScore++;
            UpdateRandomModeScoreUI();

            // Limpa os slots e o alertParent antes de gerar novas tarefas
            LimparSlots();
            LimparAlertas();

            if (fifoGenerator != null)
            {
                fifoGenerator.GenerateRandomAlerts();
                ExibirFeedback("Novo puzzle gerado!", successSound);
            }
            else
            {
                Debug.LogError("FIFOGenerator não está configurado!");
            }
        }
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

    // Método para limpar os slots
    private void LimparSlots()
    {
        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                foreach (Transform child in slot)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    // Método para limpar os alertas no alertParent
    private void LimparAlertas()
    {
        if (fifoGenerator != null && fifoGenerator.alertParent != null)
        {
            foreach (Transform child in fifoGenerator.alertParent)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("alertParent não está configurado no FIFOGenerator!");
        }
    }
}
