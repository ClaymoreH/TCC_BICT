using System.Collections.Generic;
using UnityEngine;

public class DeliverItem : MonoBehaviour
{
    public int objectID;

    [System.Serializable]
    
    public class RequiredItem
    {
        public int itemID;         
        public int requiredAmount;  
    }

    public List<RequiredItem> requiredItems;
    private bool isPlayerInArea = false;     

    private void Update()
    {
        if (isPlayerInArea && Input.GetKeyDown(KeyCode.E))
        {
            TryDeliverItems();
        }
    }

    public void TryDeliverItems()
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        if (questManager == null || inventoryManager == null)
        {
            Debug.LogError("QuestManager ou InventoryManager não encontrado!");
            return;
        }

        // Verificar se todos os itens necessários estão no inventário
        foreach (var requiredItem in requiredItems)
        {
            Item item = inventoryManager.GetItemByID(requiredItem.itemID);
            if (item == null || item.quantity < requiredItem.requiredAmount)
            {
                Debug.Log($"Você não possui os itens suficientes para entregar '{requiredItem.itemID}'.");
                return;
            }
        }

        // Se todos os itens estiverem disponíveis, remova-os do inventário e conclua a missão
        foreach (var requiredItem in requiredItems)
        {
            Item item = inventoryManager.GetItemByID(requiredItem.itemID);
            if (item != null)
            {
                item.quantity -= requiredItem.requiredAmount;
                if (item.quantity <= 0)
                {
                    inventoryManager.RemoveItem(item);
                }
            }
        }

        // Atualizar o progresso da missão
        foreach (var requiredItem in requiredItems)
        {
            questManager.UpdateQuestProgress(objectID, ObjectiveType.DeliverItem, requiredItem.requiredAmount);
            
        }

        Debug.Log("Todos os itens foram entregues! Objetivo concluído!");
        Destroy(gameObject); // Remove o objeto da cena após a entrega
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInArea = true;
            Debug.Log("Jogador entrou na área de entrega.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInArea = false;
            Debug.Log("Jogador saiu da área de entrega.");
        }
    }
}
