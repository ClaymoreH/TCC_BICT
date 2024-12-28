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

            // Adicionar o item ao inventário
            inventoryManager.AddItem(item);
            Debug.Log($"Item '{item.itemName}' adicionado ao inventário.");

            questManager.UpdateQuestProgress(item.itemID, ObjectiveType.CollectItem, item.quantity);

            Destroy(gameObject);
        }
    }
}
