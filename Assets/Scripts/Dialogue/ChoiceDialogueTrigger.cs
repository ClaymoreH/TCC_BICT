using UnityEngine;
using System.Collections;

public class ChoiceDialogueTrigger : InteractiveDialogueTrigger
{
    private InventoryManager inventoryManager;
    private DialogueUI dialogueUI;
    private QuestManager questManager;

    void Start()
    {
        base.Start();
        inventoryManager = FindObjectOfType<InventoryManager>();
        questManager = FindObjectOfType<QuestManager>();
        dialogueUI = FindObjectOfType<DialogueUI>();
    }
    public override void StartDialogue()
    {
        base.StartDialogue();  // Chama o método Start do InteractiveDialogueTrigger
        
    }
    public void CollectItem(int ActionID)
    {
        if (inventoryManager != null)
        {
            Item itemToAdd = inventoryManager.FindItemByID(ActionID);

            if (itemToAdd != null)
            {
                inventoryManager.AddItem(itemToAdd);
                Debug.Log("Item '" + itemToAdd.itemName + "' adicionado ao inventário.");
            }
        }
        Destroy(gameObject);
    }

    public void InteractWithObject(int ActionID)
    {
        if (questManager != null)
        {
            questManager.UpdateQuestProgress(ActionID, ObjectiveType.InteractWithObject);
        }
        Destroy(gameObject);
    }

public void DeliverItem(int ActionID)
{
    DeliverItem deliverItem = FindObjectOfType<DeliverItem>();

    if (deliverItem != null)
    {
        // Verifique se o ActionID corresponde a algum item na lista
        var requiredItem = deliverItem.requiredItems.Find(item => item.itemID == ActionID);

        if (requiredItem != null)
        {
            deliverItem.TryDeliverItems(); // Chama o método para verificar e entregar os itens
        }
        else
        {
            Debug.LogWarning("Nenhum item correspondente ao ActionID foi encontrado na lista de entrega.");
        }
    }
    else
    {
        Debug.LogError("Nenhum objeto DeliverItem foi encontrado na cena.");
    }
    Destroy(gameObject);
}


public void OpenPuzzle(int ActionID)
{
    PuzzleObjectData[] puzzles = FindObjectsOfType<PuzzleObjectData>(true); // Inclui objetos desativados

    foreach (PuzzleObjectData puzzle in puzzles)
    {
        if (puzzle.PainelPuzzle == ActionID)
        {
            if (puzzle.Painel != null)
            {
                puzzle.Painel.SetActive(true);
                Debug.Log($"Painel com ID {ActionID} foi ativado.");
                return;
            }
            else
            {
                Debug.LogWarning($"Objeto PuzzleObjectData com ID {ActionID} encontrado, mas o painel não foi atribuído.");
            }
        }
    }
    Destroy(gameObject);
    Debug.LogWarning($"Nenhum painel correspondente ao ID {ActionID} foi encontrado.");
}


}
