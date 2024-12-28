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

        // Verifica se o item já existe no inventário
        Item existingItem = inventoryItems.Find(i => i.itemID == item.itemID);

        if (existingItem != null)
        {
            // Se o item já existe, incrementa a quantidade em 1
            existingItem.quantity += 1;  // Incrementa sempre +1 na quantidade
        }
        else
        {
            // Se o item não existe, adiciona ao inventário com a quantidade inicial igual a 1
            item.quantity = 1;  // Define a quantidade como 1 quando o item for adicionado pela primeira vez
            inventoryItems.Add(item);
        }

        // Instancia o item no painel de inventário, fazendo com que ele seja filho de um slot vazio
        foreach (Transform slot in slotsParent)
        {
            if (slot.childCount == 0)  // Verifica se o slot está vazio
            {
                if (itemPrefab == null || item.itemSprite == null)
                {
                    Debug.LogError("ItemPrefab ou itemSprite está nulo! Verifique as configurações no Inspector.");
                    return;
                }

                // Instancia o prefab no slot
                GameObject newItem = Instantiate(itemPrefab, slot);

                // Configurar a imagem do item
                Image itemImage = newItem.GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.sprite = item.itemSprite;  // Define a sprite do item
                }
                else
                {
                    Debug.LogError("Prefab não contém um componente Image!");
                    Destroy(newItem);
                    return;
                }

                // Configurar o texto da quantidade (se existir)
                TextMeshProUGUI quantityText = newItem.GetComponentInChildren<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";  // Exibe a quantidade, se maior que 1
                }
                else
                {
                    Debug.LogWarning("Prefab não contém um componente TextMeshProUGUI! Quantidade não será exibida.");
                }

                // Configurar o nome do item
                newItem.name = item.itemName;

                // Atualizar o progresso da missão
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
            inventoryItems.Remove(item);
            UpdateInventoryUI(); 
        }
    }


    private void UpdateInventoryUI()
    {
        foreach (Transform slot in slotsParent)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        foreach (var item in inventoryItems)
        {
            GameObject newItem = Instantiate(itemPrefab, slotsParent);
            newItem.transform.SetParent(slotsParent, false);

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
        }
    }

    public Item GetItemByID(int itemID)
    {
        return inventoryItems.Find(item => item.itemID == itemID);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}
