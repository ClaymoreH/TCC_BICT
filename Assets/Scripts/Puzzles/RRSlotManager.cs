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

    void Start()
    {
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
    }

    public void OnObjectDropped(GameObject droppedObject)
    {
        if (objectsAlreadyAdded.Contains(droppedObject)) return;

        PuzzleObjectData objectData = droppedObject.GetComponent<PuzzleObjectData>();
        if (objectData == null) return;

        // Configura o PointerEventData para checar o objeto sob o mouse
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        // Armazena os resultados do raycast
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        // Verifica se o objeto foi solto sobre um slot válido
        foreach (RaycastResult result in raycastResults)
        {
            SlotIdentifier slotIdentifier = result.gameObject.GetComponent<SlotIdentifier>();
            if (slotIdentifier != null)
            {
                // Se o drop for válido, adicione o objeto ao processo e à tabela
                int processo = objectData.processo;
                float executionTime = objectData.tempoExecucao;

                AddObjectToProcess(processo, droppedObject);
                CloneObjectToColumns(droppedObject, processo, executionTime);
                objectsAlreadyAdded.Add(droppedObject);

                return; // Sai do método assim que o objeto é adicionado
            }
        }

        Debug.Log("Objeto não foi solto em uma área válida de drop.");
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
                Debug.Log($"Sem slots disponíveis para o Processo {processo}.");
                return;
            }

            GameObject clonedObject = CloneObject(obj);
            PlaceObjectInSlot(clonedObject, nextSlot);
        }
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

    private GameObject FindNextAvailableSlot(int processo)
    {
        foreach (GameObject slot in slots)
        {
            SlotIdentifier slotIdentifier = slot.GetComponent<SlotIdentifier>();
            if (slotIdentifier != null && IsSlotAvailableForProcess(slotIdentifier, processo))
            {
                lastUsedColumn = slotIdentifier.column + 1; // Atualiza a última coluna usada
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

    public void ResetTable()
    {
        ClearSlots();
        objectsAlreadyAdded.Clear();
        processObjects.Clear();
        lastUsedColumn = 1;
    }

    private void ClearSlots()
    {
        foreach (GameObject slot in slots)
        {
            Transform child = slot.transform.GetChild(0);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
