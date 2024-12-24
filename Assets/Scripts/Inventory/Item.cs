using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; // Nome do item
    public Sprite itemSprite; // Ícone do item
    public int quantity; // Quantidade do item
}
