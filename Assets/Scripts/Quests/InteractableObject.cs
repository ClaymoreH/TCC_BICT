using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int objectID;

    public void Interact()
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager == null)
        {
            Debug.LogError("QuestManager não encontrado na cena!");
            return;
        }

        // Verificar se o objetivo de interação pode ser completado
        if (questManager.CanCompleteObjective(objectID, ObjectiveType.InteractWithObject))
        {
            Debug.Log($"Interagiu com o objeto: {objectID}");

            // Atualizar o progresso da missão
            questManager.UpdateQuestProgress(objectID, ObjectiveType.InteractWithObject);

            // Destruir o objeto interativo, caso aplicável
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Você não pode interagir com o objeto {objectID} ainda. Complete os objetivos anteriores.");
        }
    }
}
