using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            QuestManager questManager = FindObjectOfType<QuestManager>();
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

            if (questManager == null || inventoryManager == null)
            {
                Debug.LogError("QuestManager ou InventoryManager não encontrado na cena!");
                return;
            }

            // Verificar se o objetivo pode ser completado
            if (questManager.CanCompleteObjective(item.itemID, ObjectiveType.CollectItem))
            {
                // Adicionar o item ao inventário
                inventoryManager.AddItem(item);
                Debug.Log($"Item '{item.itemName}' adicionado ao inventário.");

                // Atualizar o progresso da missão
                questManager.UpdateQuestProgress(item.itemID, ObjectiveType.CollectItem, item.quantity);

                // Destruir o objeto coletável
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Não é possível coletar '{item.itemName}' ainda. Complete os objetivos anteriores.");
            }
        }
    }
}
