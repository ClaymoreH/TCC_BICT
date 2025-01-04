using UnityEngine;

public class DeliverItem : MonoBehaviour
{
    public int objectID;             // ID do objeto (correspondente ao objetivo)
    public int requiredItemID;       // ID do item necessário para entregar
    public int requiredItemAmount;   // Quantidade necessária do item
    private bool isPlayerInArea = false; // Flag para verificar se o jogador está na área de entrega

    private void Update()
    {
        if (isPlayerInArea && Input.GetKeyDown(KeyCode.E)) // Verificar se o jogador está na área e tecla "E" é pressionada
        {
            TryDeliverItem();
        }
    }

    private void TryDeliverItem()
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        if (questManager == null || inventoryManager == null)
        {
            Debug.LogError("QuestManager ou InventoryManager não encontrado na cena!");
            return;
        }

        // Verificar se o objetivo de entrega pode ser completado
        if (questManager.CanCompleteObjective(objectID, ObjectiveType.DeliverItem))
        {
            Item item = inventoryManager.GetItemByID(requiredItemID);

            if (item != null && item.quantity >= requiredItemAmount)
            {
                // Atualizar quantidade no inventário
                item.quantity -= requiredItemAmount;

                if (item.quantity == 0)
                {
                    inventoryManager.RemoveItem(item);
                }

                // Atualizar o progresso da missão
                questManager.UpdateQuestProgress(objectID, ObjectiveType.DeliverItem, requiredItemAmount);

                Debug.Log($"Item '{item.itemName}' entregue. Objetivo concluído!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Não há itens suficientes de '{requiredItemID}' no inventário.");
            }
        }
        else
        {
            Debug.Log($"Você não pode entregar o item {requiredItemID} ainda. Complete os objetivos anteriores.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Verificar se o jogador entrou na área
        {
            isPlayerInArea = true;
            Debug.Log("Jogador entrou na área de entrega.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Verificar se o jogador saiu da área
        {
            isPlayerInArea = false;
            Debug.Log("Jogador saiu da área de entrega.");
        }
    }
}
