using UnityEngine;
using System.Collections;

public class ChoiceDialogueTrigger : InteractiveDialogueTrigger
{
    private InventoryManager inventoryManager;
    private QuestManager questManager;

    void Start()
    {
        base.Start();
        inventoryManager = FindObjectOfType<InventoryManager>();
        questManager = FindObjectOfType<QuestManager>();
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
            deliverItem.objectID = ActionID; // Define o ID do objeto para entrega
            deliverItem.TryDeliverItem();   // Tenta realizar a entrega
        }
        Destroy(gameObject);
    }

public void OpenPuzzle(int ActionID)
{
    // Lógica para tornar o quebra-cabeça correspondente ao ActionID visível
    Debug.Log("Quebra-cabeça com ID " + ActionID + " será mostrado.");

    // Procurar todos os objetos do tipo PuzzleObjectData na cena
    PuzzleObjectData[] puzzleObjects = FindObjectsOfType<PuzzleObjectData>();

    foreach (PuzzleObjectData puzzleObject in puzzleObjects)
    {
        // Verifica se o PainelPuzzle do objeto é igual ao ActionID
        if (puzzleObject.PainelPuzzle == ActionID)
        {
            // Tenta encontrar o painel correspondente
            GameObject puzzlePanel = GameObject.Find("PuzzlePanel" + puzzleObject.PainelPuzzle); // Certifique-se de que os nomes dos painéis seguem o padrão

            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(true); // Torna o painel visível
                Debug.Log("Painel de quebra-cabeça com ID " + puzzleObject.PainelPuzzle + " foi tornado visível.");
                break; // Já encontrou o painel, então quebra o loop
            }
            else
            {
                Debug.LogWarning("Painel de quebra-cabeça com ID " + puzzleObject.PainelPuzzle + " não encontrado.");
            }
        }
    }

    Destroy(gameObject); // Destroi o objeto de diálogo, como no código original
}

}
