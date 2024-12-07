using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    private int currentRow = 1;
    private int currentColumn = 1;
    private bool isTableFull = false;

    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();
    private Dictionary<GameObject, List<GameObject>> groupedObjects = new Dictionary<GameObject, List<GameObject>>();

    // Armazena a última coluna usada em cada linha.
    private Dictionary<int, int> lastColumnInRow = new Dictionary<int, int>();

    void Start()
    {
        // Acessa o evento de drop do DragAndDrop2D
        DragAndDrop2D.OnDrop += OnObjectDropped;

        tableGenerator.OnTableGenerated += PopulateSlots;
        tableGenerator.GenerateTable(tableGenerator.rows, tableGenerator.columns);
    }

    private void PopulateSlots()
    {
        slots.Clear();

        foreach (Transform child in tableGenerator.parentPanel)
        {
            slots.Add(child.gameObject);
        }

        if (slots.Count == 0)
        {
            Debug.LogError("Nenhum slot encontrado na tabela!");
        }
    }

    private void OnObjectDropped(GameObject droppedObject)
    {
        if (isTableFull)
        {
            Debug.LogWarning("A tabela está cheia. Não é possível adicionar novos objetos.");
            return;
        }

        if (objectsAlreadyAdded.Contains(droppedObject))
        {
            MoveGroupedObjects(droppedObject);
            return;
        }

        List<GameObject> newGroup = new List<GameObject>();
        groupedObjects.Add(droppedObject, newGroup);

        float tempoExecucao = droppedObject.GetComponent<PuzzleObjectData>()?.tempoExecucao ?? 1;

        // Se for o primeiro objeto, utiliza a posição atual
        for (int i = 0; i < tempoExecucao; i++)
        {
            GameObject nextSlot = FindNextAvailableSlot();
            if (nextSlot != null)
            {
                GameObject clone = i == 0 ? droppedObject : Instantiate(droppedObject, transform);
                clone.transform.SetParent(nextSlot.transform);
                clone.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                clone.GetComponent<RectTransform>().localScale = Vector3.one;

                objectsAlreadyAdded.Add(clone);
                newGroup.Add(clone);

                // Atualiza a posição para o próximo slot
                CalculateNextPosition();

                if (currentColumn > tableGenerator.columns)
                {
                    isTableFull = true;
                    Debug.Log("A tabela está cheia! Não é possível adicionar mais objetos.");
                    break;
                }
            }
        }

        // Após adicionar o objeto, atualiza a linha e a coluna
        if (!lastColumnInRow.ContainsKey(currentRow))
        {
            lastColumnInRow[currentRow] = currentColumn - 1;  // Armazena a última coluna preenchida na linha
        }

        // Atualiza a linha e a coluna para o próximo objeto
        currentRow++;
        currentColumn = lastColumnInRow.ContainsKey(currentRow - 1) ? lastColumnInRow[currentRow - 1] + 1 : 1;
    }

    private void MoveGroupedObjects(GameObject droppedObject)
    {
        if (groupedObjects.TryGetValue(droppedObject, out List<GameObject> group))
        {
            GameObject currentSlot = FindSlot(currentRow, currentColumn);

            if (currentSlot != null)
            {
                foreach (GameObject obj in group)
                {
                    obj.transform.SetParent(currentSlot.transform);
                    obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    obj.GetComponent<RectTransform>().localScale = Vector3.one;

                    int index = group.IndexOf(obj);
                    if (index > 0)
                    {
                        float offsetX = (obj.GetComponent<RectTransform>().rect.width + 5) * index;
                        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetX, 0);

                        if (currentColumn + index <= tableGenerator.columns)
                        {
                            CalculateNextPosition();
                            currentSlot = FindSlot(currentRow, currentColumn);
                        }
                        else
                        {
                            // Se ultrapassar o limite da coluna, reinicia na próxima linha
                            currentRow++;
                            currentColumn = index;
                            currentSlot = FindSlot(currentRow, currentColumn);
                        }
                    }
                    else
                    {
                        CalculateNextPosition();
                        currentSlot = FindSlot(currentRow, currentColumn);
                    }
                }
            }
        }
    }

    private void CalculateNextPosition()
    {
        currentColumn++;

        // Se a coluna for maior que o número de colunas, vai para a próxima linha
        if (currentColumn > tableGenerator.columns)
        {
            currentColumn = 1;
            currentRow++;
        }
    }

    private GameObject FindSlot(int row, int column)
    {
        foreach (GameObject slot in slots)
        {
            SlotIdentifier identifier = slot.GetComponent<SlotIdentifier>();
            if (identifier != null && identifier.row == row && identifier.column == column)
            {
                return slot;
            }
        }
        return null;
    }

    private GameObject FindNextAvailableSlot()
    {
        GameObject slot = FindSlot(currentRow, currentColumn);
        if (slot != null)
        {
            return slot;
        }

        Debug.LogWarning("Não foi possível encontrar um próximo slot disponível.");
        return null;
    }
}
