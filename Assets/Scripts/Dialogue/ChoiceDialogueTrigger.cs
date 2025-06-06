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
}


    public void InteractWithObject(int ActionID)
    {
        if (questManager != null)
        {
            questManager.UpdateQuestProgress(ActionID, ObjectiveType.InteractWithObject);
        }

    }

public void DeliverItem(int ActionID)
{
    // Encontra todos os DeliverItems na cena
    DeliverItem[] deliverItems = FindObjectsOfType<DeliverItem>();

    // Exibe o número de DeliverItems encontrados
    Debug.Log($"Encontrados {deliverItems.Length} objetos DeliverItem na cena.");

    DeliverItem foundDeliverItem = null;
    foreach (DeliverItem item in deliverItems)
    {
        if (item.objectID == ActionID)
        {
            foundDeliverItem = item;
            break; // Encontrou o item, não precisa continuar a busca
        }
    }

    // Se encontrou o DeliverItem, tenta entregar os itens
    if (foundDeliverItem != null)
    {
        foundDeliverItem.TryDeliverItems();
        Debug.Log($"Itens entregues com sucesso para o objeto de ID {ActionID}.");
    }
    else
    {
        Debug.LogWarning($"Nenhum objeto de entrega correspondente ao ID {ActionID} foi encontrado.");
    }
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

        // Destrói o objeto que iniciou a interação
        Debug.LogWarning($"Nenhum painel correspondente ao ID {ActionID} foi encontrado.");
    }
}
