using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public int puzzleID; // ID único para identificar o puzzle

    public void CompletePuzzle()
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager == null)
        {
            Debug.LogError("QuestManager não encontrado na cena!");
            return;
        }

        // Verificar se o objetivo do puzzle pode ser completado
        if (questManager.CanCompleteObjective(puzzleID, ObjectiveType.SolvePuzzle))
        {
            Debug.Log($"Puzzle {puzzleID} concluído!");

            // Atualizar o progresso da missão
            questManager.UpdateQuestProgress(puzzleID, ObjectiveType.SolvePuzzle);
        }
        else
        {
            Debug.Log($"Você não pode resolver o puzzle {puzzleID} ainda. Complete os objetivos anteriores.");
        }
    }
}
