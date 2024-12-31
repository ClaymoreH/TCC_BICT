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

            // Verificar se o objetivo pode ser completado
            if (questManager.CanCompleteObjective(locationID, ObjectiveType.ReachLocation))
            {
                questManager.UpdateQuestProgress(locationID, ObjectiveType.ReachLocation);
                Debug.Log($"Chegou ao local: {locationID}");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Você não pode completar este objetivo ainda. Verifique os objetivos anteriores.");
            }
        }
    }
}
