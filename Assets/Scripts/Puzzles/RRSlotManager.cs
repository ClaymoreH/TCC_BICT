using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RRSlotManager : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    public int Quantum = 3; // Determina quantas colunas cada processo ocupa
    private Dictionary<int, List<GameObject>> processObjects = new Dictionary<int, List<GameObject>>();
    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();
    private int lastUsedColumn = 1;
    public int tableID;  // ID único para identificar a tabela

    void Start()
    {
        // Gerar um ID único para a tabela
        tableID = UnityEngine.Random.Range(1, 10000); 
        tableGenerator.SetTableID(tableID);

        // Assinar eventos locais
        tableGenerator.OnTableGenerated += PopulateSlots;
        GenerateTable();

        // Assinar o evento OnDrop apenas para esta tabela
        DragAndDrop2D.OnDrop += OnObjectDropped;
    }

    private void OnDestroy()
    {
        // Cancelar inscrição para evitar referências a objetos destruídos
        DragAndDrop2D.OnDrop -= OnObjectDropped;
    }

    private void GenerateTable()
    {
        tableGenerator.GenerateTable(tableGenerator.rows, tableGenerator.columns);
    }

    private void PopulateSlots()
    {
        slots.Clear();
        foreach (Transform child in tableGenerator.parentPanel)
        {
            slots.Add(child.gameObject);
        }
    }

    public void OnObjectDropped(GameObject droppedObject)
    {
        // Verificar se o objeto foi solto em uma DropZone válida e pertencente a esta tabela
        if (objectsAlreadyAdded.Contains(droppedObject)) return;

        PuzzleObjectData objectData = droppedObject.GetComponent<PuzzleObjectData>();
        if (objectData == null) return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        bool isValidDrop = false;

        foreach (RaycastResult result in raycastResults)
        {
            SlotIdentifier slotIdentifier = result.gameObject.GetComponent<SlotIdentifier>();
            if (slotIdentifier != null && slotIdentifier.tableID == tableID)
            {
                // Validação: Apenas processar se o slot pertence à tabela correta
                isValidDrop = true;
                break;  // Se encontrar um slot válido, sai do loop
            }

            // Verificar se o objeto foi solto em uma CircularDropZone
            CircularDropZone circularDropZone = result.gameObject.GetComponent<CircularDropZone>();
            if (circularDropZone != null)
            {
                isValidDrop = true;
                break;  // Se encontrar a CircularDropZone, sai do loop
            }
        }

        if (isValidDrop)
        {
            int processo = objectData.processo;
            float executionTime = objectData.tempoExecucao;

            AddObjectToProcess(processo, droppedObject);
            CloneObjectToColumns(droppedObject, processo, executionTime);
            objectsAlreadyAdded.Add(droppedObject);
        }
        else
        {
            Debug.Log("Objeto não foi solto em uma área válida de drop para esta tabela.");
        }
    }

    private void AddObjectToProcess(int processo, GameObject droppedObject)
    {
        if (!processObjects.ContainsKey(processo))
        {
            processObjects[processo] = new List<GameObject>();
        }
        processObjects[processo].Add(droppedObject);
    }

    private void CloneObjectToColumns(GameObject obj, int processo, float executionTime)
    {
        int columnsToOccupy = Mathf.Min(Quantum, Mathf.CeilToInt(executionTime));
        for (int i = 0; i < columnsToOccupy; i++)
        {
            GameObject nextSlot = FindNextAvailableSlot(processo);
            if (nextSlot == null)
            {
                Debug.Log($"Sem slots disponíveis para o Processo {processo} na Tabela {tableID}.");
                return;
            }

            GameObject clonedObject = CloneObject(obj);
            PlaceObjectInSlot(clonedObject, nextSlot);
        }
    }

    private GameObject FindNextAvailableSlot(int processo)
    {
        foreach (GameObject slot in slots)
        {
            SlotIdentifier slotIdentifier = slot.GetComponent<SlotIdentifier>();
            if (slotIdentifier != null && 
                slotIdentifier.tableID == tableID &&  // Filtra pela tabela correta
                IsSlotAvailableForProcess(slotIdentifier, processo))
            {
                lastUsedColumn = slotIdentifier.column + 1;
                return slot;
            }
        }
        return null;
    }

    private bool IsSlotAvailableForProcess(SlotIdentifier slotIdentifier, int processo)
    {
        return slotIdentifier.row == processo &&
               slotIdentifier.column >= lastUsedColumn &&
               slotIdentifier.transform.childCount == 0;
    }

    private GameObject CloneObject(GameObject obj)
    {
        GameObject clonedObject = Instantiate(obj);
        return clonedObject;
    }

    private void PlaceObjectInSlot(GameObject obj, GameObject slot)
    {
        obj.transform.SetParent(slot.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.GetComponent<RectTransform>().localScale = Vector3.one;
    }
}
