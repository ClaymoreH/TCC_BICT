using UnityEngine;

public class ReachLocation : MonoBehaviour
{
    public int locationID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager == null)
            {
                Debug.LogError("QuestManager não encontrado na cena!");
                return;
            }

            questManager.UpdateQuestProgress(locationID, ObjectiveType.ReachLocation);
            Debug.Log($"Chegou ao local: {locationID}");
            Destroy(gameObject);
        }
    }

}
