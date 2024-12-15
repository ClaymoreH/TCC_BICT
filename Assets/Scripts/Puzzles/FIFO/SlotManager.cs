using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotManager : MonoBehaviour
{
    public int tableID; // Identificador único da tabela
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    private int currentRow = 1;
    private int currentColumn = 1;
    private bool isTableFull = false;
    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();

    public Transform parentOriginal; // Pai original dos objetos
    private Dictionary<int, int> lastColumnInRow = new Dictionary<int, int>(); // Última coluna ocupada em cada linha
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); // Posições originais dos objetos

    void Start()
    {
        // Registra eventos e cria a tabela
        DragAndDrop2D.OnDrop += OnObjectDropped;
        tableGenerator.OnTableGenerated += PopulateSlots;
        tableGenerator.GenerateTable(tableGenerator.rows, tableGenerator.columns);
    }

    // Popula a lista de slots filtrando pelo tableID
    private void PopulateSlots()
    {
        slots.Clear();
        foreach (Transform child in tableGenerator.parentPanel)
        {
            SlotIdentifier identifier = child.GetComponent<SlotIdentifier>();
            if (identifier != null && identifier.tableID == tableID) // Verifica se o slot pertence à tabela correta
            {
                slots.Add(child.gameObject);
            }
        }
    }

    // Método chamado quando um objeto é solto
    private void OnObjectDropped(GameObject droppedObject)
    {
        if (isTableFull || objectsAlreadyAdded.Contains(droppedObject)) return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            SlotIdentifier slotIdentifier = result.gameObject.GetComponent<SlotIdentifier>();
            if (slotIdentifier != null && slotIdentifier.tableID == tableID) // Garante que o slot pertence à tabela atual
            {
                AddObjectToTable(droppedObject);
                return;
            }
        }
    }

    // Adiciona o objeto à tabela
    private void AddObjectToTable(GameObject droppedObject)
    {
        float executionTime = droppedObject.GetComponent<PuzzleObjectData>()?.tempoExecucao ?? 1;

        originalPositions[droppedObject] = droppedObject.transform.position;

        for (int i = 0; i < executionTime; i++)
        {
            GameObject nextSlot = FindNextAvailableSlot();
            if (nextSlot != null)
            {
                GameObject clone = i == 0 ? droppedObject : Instantiate(droppedObject, transform);
                PlaceObjectInSlot(clone, nextSlot);
                objectsAlreadyAdded.Add(clone);
                MoveToNextPosition();

                if (currentRow > tableGenerator.rows)
                {
                    isTableFull = true;
                    break;
                }
            }
            else
            {
                isTableFull = true;
                break;
            }
        }

        if (!lastColumnInRow.ContainsKey(currentRow))
        {
            lastColumnInRow[currentRow] = currentColumn - 1;
        }

        currentRow++;
        currentColumn = lastColumnInRow.ContainsKey(currentRow - 1) ? lastColumnInRow[currentRow - 1] + 1 : 1;
    }

    // Coloca o objeto no slot
    private void PlaceObjectInSlot(GameObject obj, GameObject slot)
    {
        obj.transform.SetParent(slot.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    // Atualiza a posição atual na tabela
    private void MoveToNextPosition()
    {
        currentColumn++;
        if (currentColumn > tableGenerator.columns)
        {
            currentColumn = 1;
            currentRow++;
        }
    }

    // Encontra um slot específico
    public GameObject FindSlot(int row, int column)
    {
        foreach (GameObject slot in slots)
        {
            SlotIdentifier identifier = slot.GetComponent<SlotIdentifier>();
            if (identifier != null && identifier.row == row && identifier.column == column && identifier.tableID == tableID)
            {
                return slot;
            }
        }
        return null;
    }

    // Encontra o próximo slot disponível
    private GameObject FindNextAvailableSlot()
    {
        return FindSlot(currentRow, currentColumn);
    }

    // Reseta a tabela
    public void ResetTable()
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                if (originalPositions.ContainsKey(child.gameObject))
                {
                    child.SetParent(parentOriginal);
                    child.position = originalPositions[child.gameObject];
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }

        objectsAlreadyAdded.Clear();
        originalPositions.Clear();
        currentRow = 1;
        currentColumn = 1;
        isTableFull = false;
        lastColumnInRow.Clear();

        Debug.Log($"Tabela {tableID} resetada e pronta para reutilização.");
    }
}
