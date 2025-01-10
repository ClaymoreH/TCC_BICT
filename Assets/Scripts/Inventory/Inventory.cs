using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; 
    public Transform slotsParent;  
    public GameObject itemPrefab;  
    public List<Item> inventoryItems = new List<Item>(); 

    void Start()
    {
        inventoryPanel.SetActive(false); 
    }

    public void AddItem(Item item)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();

        Item existingItem = inventoryItems.Find(i => i.itemID == item.itemID);

        if (existingItem != null)
        {
            existingItem.quantity += 1;
        }
        else
        {
            item.quantity = 1;
            inventoryItems.Add(item);
        }

        foreach (Transform slot in slotsParent)
        {
            if (slot.childCount == 0)
            {
                if (itemPrefab == null || item.itemSprite == null)
                {
                    Debug.LogError("ItemPrefab ou itemSprite está nulo! Verifique as configurações no Inspector.");
                    return;
                }

                GameObject newItem = Instantiate(itemPrefab, slot);

                Image itemImage = newItem.GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.sprite = item.itemSprite;
                }
                else
                {
                    Debug.LogError("Prefab não contém um componente Image!");
                    Destroy(newItem);
                    return;
                }

                TextMeshProUGUI quantityText = newItem.GetComponentInChildren<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
                }
                else
                {
                    Debug.LogWarning("Prefab não contém um componente TextMeshProUGUI! Quantidade não será exibida.");
                }

                newItem.name = item.itemName;

                if (questManager != null)
                {
                    questManager.UpdateQuestProgress(item.itemID, ObjectiveType.CollectItem, item.quantity);
                }

                Debug.Log($"Item '{item.itemName}' adicionado ao inventário.");
                return;
            }
        }

        Debug.Log("Inventário cheio! Não foi possível adicionar o item.");
    }

public void RemoveItem(Item item)
{
    if (inventoryItems.Contains(item))
    {
        // Reduz a quantidade do item
        item.quantity--;

        // Remove o item da lista se a quantidade chegar a 0
        if (item.quantity <= 0)
        {
            inventoryItems.Remove(item);
        }

        // Atualiza a interface do inventário
        UpdateInventoryUI();
    }
    else
    {
        Debug.LogWarning($"Item '{item.itemName}' não encontrado no inventário para remoção.");
    }
}

public void RemoveItems(List<Item> itemsToRemove)
{
    foreach (Item item in itemsToRemove)
    {
        RemoveItem(item); // Usa o método individual para garantir consistência
    }
}

private void UpdateInventoryUI()
{
    // Limpa todos os slots
    foreach (Transform slot in slotsParent)
    {
        if (slot.childCount > 0)
        {
            Destroy(slot.GetChild(0).gameObject); // Destrói os objetos dentro dos slots
        }
    }

    // Recria os itens do inventário na interface
    foreach (var item in inventoryItems)
    {
        foreach (Transform slot in slotsParent)
        {
            if (slot.childCount == 0) // Encontra um slot vazio
            {
                GameObject newItem = Instantiate(itemPrefab, slot);
                
                Image itemImage = newItem.GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.sprite = item.itemSprite;
                }

                TextMeshProUGUI quantityText = newItem.GetComponentInChildren<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
                }

                newItem.name = item.itemName;
                break; // Sai do loop após adicionar o item a um slot
            }
        }
    }
}

    public Item GetItemByID(int itemID)
    {
        return inventoryItems.Find(item => item.itemID == itemID);
    }
    // Método para buscar o item diretamente nas pastas de Resources
    public Item FindItemByID(int itemID)
    {
        // Carregar todos os itens da pasta "Items" dentro da pasta Resources
        Item[] allItems = Resources.LoadAll<Item>("Items");

        // Procurar o item pelo itemID
        foreach (Item item in allItems)
        {
            if (item.itemID == itemID)
            {
                return item;
            }
        }

        // Se não encontrar, retorna null
        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}
