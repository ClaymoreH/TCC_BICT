
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; 
    public Transform slotsParent; 
    public GameObject itemPrefab; 

    public void AddItem(Item item)
    {
        foreach (Transform slot in slotsParent)
        {
            if (slot.childCount == 0) 
            {
                GameObject newItem = Instantiate(itemPrefab, slot);
                newItem.GetComponent<Image>().sprite = item.itemSprite;

                TextMesh quantityText = newItem.GetComponentInChildren<TextMesh>();

                quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";

                newItem.name = item.itemName; 
                return;
            }
        }
        Debug.Log("Inventário cheio!");
    }


    public void ClearInventory()
    {
        foreach (Transform slot in slotsParent)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.gameObject.activeSelf);
        }
    }

} 