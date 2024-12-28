using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int objectID; 

    public void Interact()
    {
        Debug.Log($"Interagiu com o objeto: {objectID}");

        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.UpdateQuestProgress(objectID, ObjectiveType.InteractWithObject);
        }
        Destroy(gameObject);
    }
}
