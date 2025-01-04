using UnityEngine;
using UnityEngine.UI;
using System;

public class DynamicTableGenerator : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform parentPanel;
    public int rows = 5;
    public int columns = 15;
    public Vector2 cellSize = new Vector2(50, 50);
    public Vector2 spacing = new Vector2(5, 5);
    public int tableID;  

    public event Action OnTableGenerated;

    public void SetTableID(int id)
    {
        tableID = id;
    }

    public void GenerateTable(int rows, int columns)
    {
        foreach (Transform child in parentPanel)
        {
            Destroy(child.gameObject);
        }

        GridLayoutGroup gridLayout = parentPanel.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = parentPanel.gameObject.AddComponent<GridLayoutGroup>();
        }
        gridLayout.cellSize = cellSize;
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject slot = Instantiate(slotPrefab, parentPanel);
                slot.name = $"Slot_Linha_{i + 1}_Coluna_{j + 1}";

                SlotIdentifier slotIdentifier = slot.AddComponent<SlotIdentifier>();
                slotIdentifier.row = i + 1;
                slotIdentifier.column = j + 1;
                slotIdentifier.tableID = tableID;
            }
        }

        OnTableGenerated?.Invoke();
    }
}

[System.Serializable]
public class SlotIdentifier : MonoBehaviour
{
    public int row;
    public int column;
    public int tableID;
}
