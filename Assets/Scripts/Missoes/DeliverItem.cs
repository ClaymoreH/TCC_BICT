using UnityEngine;

public class DeliverItem : MonoBehaviour
{
    public int objectID;         // ID do objeto (correspondente ao objetivo)
    public int requiredItemID;   // ID do item necessário para entregar
    public int requiredItemAmount; // Quantidade necessária do item

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Verificar se a tecla "E" é pressionada
        {
            TryDeliverItem();
        }
    }

    void TryDeliverItem()
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager não encontrado na cena!");
            return;
        }

        Item item = inventoryManager.GetItemByID(requiredItemID);

        if (item != null && item.quantity >= requiredItemAmount)
        {
            item.quantity -= requiredItemAmount;

            if (item.quantity == 0)
            {
                inventoryManager.RemoveItem(item);
            }

            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null)
            {
                questManager.UpdateQuestProgress(requiredItemID, ObjectiveType.DeliverItem, requiredItemAmount);
            }

            Debug.Log($"Item '{item.itemName}' entregue. Objetivo concluído!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Não há itens suficientes de '{requiredItemID}' no inventário.");
        }
    }
}
