using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; 
    public int itemID;
    public Sprite itemSprite; 
    public int quantity; 
}
