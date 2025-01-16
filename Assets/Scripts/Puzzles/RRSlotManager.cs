using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class RRSlotManager : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    public int Quantum = 3;
    private Dictionary<int, List<GameObject>> processObjects = new Dictionary<int, List<GameObject>>();
    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();
    private int lastUsedColumn = 1;
    public int tableID;
    public Dictionary<GameObject, int> quantumSubtracted = new Dictionary<GameObject, int>();
    private static int nextTableID = 0;
    private static List<int> deletedTableIDs = new List<int>();
    public Dictionary<GameObject, Dictionary<int, int>> quantumSubtractedByDropZone = new Dictionary<GameObject, Dictionary<int, int>>();

    private static int GetNextTableID()
    {
        if (deletedTableIDs.Count > 0)
        {
            int availableID = deletedTableIDs.Min();
            deletedTableIDs.Remove(availableID); 
            return availableID;
        }
        else
        {
            return nextTableID++;
        }
    }

    
    void Start()
    {

        tableID = GetNextTableID(); 
        tableGenerator.SetTableID(tableID);

        tableGenerator.OnTableGenerated += PopulateSlots;
        GenerateTable();

        SetTableIDForDropZones();

        DragAndDrop2D.OnDrop += OnObjectDropped;
        
    }


    private void SetTableIDForDropZones()
    {
        CircularDropZone[] dropZones = FindObjectsOfType<CircularDropZone>();
        
        foreach (var dropZone in dropZones)
        {
            dropZone.SetTableID(tableID);
        }
    }

    private void OnDestroy()
    {
        deletedTableIDs.Add(tableID);
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

    private void UpdateLastUsedColumn()
    {
        int lastColumnFound = 0;

        foreach (GameObject slot in slots)
        {
            SlotIdentifier slotIdentifier = slot.GetComponent<SlotIdentifier>();
            
            if (slotIdentifier != null && slot.transform.childCount > 0)
            {
                // Atualiza a última coluna encontrada se for maior que o valor atual
                lastColumnFound = Mathf.Max(lastColumnFound, slotIdentifier.column);
            }
        }

        // Define a última coluna como a próxima após a maior ocupada, ou 1 se nenhuma estiver ocupada
        lastUsedColumn = lastColumnFound + 1;
    }

    public void ClearTable(int dropzoneIDToClear)
    {
        // Cria uma lista temporária com as chaves do dicionário
        List<GameObject> objectsToRestore = new List<GameObject>(quantumSubtractedByDropZone.Keys);

        // Restaura valores de todos os objetos que estavam associados a esta DropZone
        foreach (var obj in objectsToRestore)
        {
            RestoreObjectValues(obj, dropzoneIDToClear);
        }

        // Limpa os objetos da DropZone específica
        foreach (GameObject slot in slots)
        {
            bool slotHasObjectsToClear = false;

            foreach (Transform child in slot.transform)
            {
                PuzzleObjectData objectData = child.gameObject.GetComponent<PuzzleObjectData>();
                if (objectData != null && objectData.dropzoneID == dropzoneIDToClear)
                {
                    slotHasObjectsToClear = true;

                    child.SetParent(null);
                    child.gameObject.SetActive(false);
                }
            }
            if (slotHasObjectsToClear)
            {
                Image slotImage = slot.GetComponent<Image>();
                if (slotImage != null)
                {
                    slotImage.color = new Color32(0xC3, 0xC3, 0xC3, 0xFF);
                }
            }
        }

        UpdateLastUsedColumn();
        Debug.Log($"DropZone {dropzoneIDToClear} limpa e valores restaurados.");
    }

    public void RestoreObjectValues(GameObject obj, int dropzoneID)
    {
        if (quantumSubtractedByDropZone.TryGetValue(obj, out Dictionary<int, int> dropzoneQuantums))
        {
            if (dropzoneQuantums.ContainsKey(dropzoneID))
            {
                int quantumToRestore = dropzoneQuantums[dropzoneID];
                
                // Restaura o valor para o objeto
                PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
                if (objectData != null)
                {
                    objectData.tempoExecucao += quantumToRestore;
                    objectData.tempoExecucaoTotal -= quantumToRestore;

                    Debug.Log($"Quantum {quantumToRestore} restaurado para o objeto {objectData.name} na DropZone {dropzoneID}.");
                }

                // Remove apenas o quantum da DropZone específica, não excluindo tudo ainda
                dropzoneQuantums[dropzoneID] = 0; // Marque como restaurado para essa DropZone

                // Se todas as subtrações da DropZone foram restauradas, remove o objeto completamente do dicionário
                if (dropzoneQuantums.Values.All(v => v == 0))
                {
                    quantumSubtractedByDropZone.Remove(obj);
                    Debug.Log($"Todas as subtrações restauradas para o objeto {obj.name}. Removido do dicionário.");
                }
            }
        }
    }
    public void SubtractObjectValue(GameObject obj, int quantum, int dropzoneID)
    {
        if (!quantumSubtractedByDropZone.ContainsKey(obj))
        {
            quantumSubtractedByDropZone[obj] = new Dictionary<int, int>();
        }

        if (!quantumSubtractedByDropZone[obj].ContainsKey(dropzoneID))
        {
            quantumSubtractedByDropZone[obj][dropzoneID] = 0;
        }

        quantumSubtractedByDropZone[obj][dropzoneID] += quantum;

        PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
        if (objectData != null)
        {
            objectData.tempoExecucao -= quantum;
            objectData.tempoExecucaoTotal += quantum;

            Debug.Log($"Quantum {quantum} subtraído de {objectData.name} pela DropZone {dropzoneID}.");
        }
    }

    public void OnObjectDropped(GameObject droppedObject)
    {

        PuzzleObjectData objectData = droppedObject.GetComponent<PuzzleObjectData>();
        if (objectData == null) return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        CircularDropZone targetDropZone = null;
        bool isValidDrop = false;

        foreach (RaycastResult result in raycastResults)
        {
            CircularDropZone dropZone = result.gameObject.GetComponent<CircularDropZone>();
            if (dropZone != null && dropZone.tableID == tableID)
            {
                targetDropZone = dropZone;
                isValidDrop = true;
                break;
            }
        }

        if (isValidDrop && targetDropZone != null)
        {
            Debug.Log($"Objeto solto na tabela {targetDropZone.tableID} associada ao CircularDropZone.");
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

        PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
        if (objectData != null)
        {
            int previousTempo = objectData.tempoExecucao;

            int subtracted = Mathf.Min(Quantum, previousTempo);
            objectData.tempoExecucao = Mathf.Max(0, previousTempo - Quantum);

            // Armazenar o quantum subtraído por dropzone
            if (!quantumSubtractedByDropZone.ContainsKey(obj))
            {
                quantumSubtractedByDropZone[obj] = new Dictionary<int, int>();
            }

            if (!quantumSubtractedByDropZone[obj].ContainsKey(objectData.dropzoneID))
            {
                quantumSubtractedByDropZone[obj][objectData.dropzoneID] = 0;
            }

            quantumSubtractedByDropZone[obj][objectData.dropzoneID] += subtracted;

            objectData.tempoExecucaoTotal += subtracted;
            Debug.Log($"Tempo total executado para {objectData.name}: {objectData.tempoExecucaoTotal}");
        }

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
                slotIdentifier.tableID == tableID && 
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

        Image slotImage = slot.GetComponent<Image>();
        Image objImage = obj.GetComponent<Image>();

        if (slotImage != null && objImage != null)
        {
            slotImage.color = objImage.color;

            objImage.enabled = false;
        }
    }

}
