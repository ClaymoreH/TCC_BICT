using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class RRSlotManager : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    public int Quantum = 3; // Determina quantas colunas cada processo ocupa
    private Dictionary<int, List<GameObject>> processObjects = new Dictionary<int, List<GameObject>>();
    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();
    private int lastUsedColumn = 1;
    public int tableID;  // ID único para identificar a tabela
    public Dictionary<GameObject, int> quantumSubtracted = new Dictionary<GameObject, int>();
    private static int nextTableID = 1;
    private static List<int> deletedTableIDs = new List<int>();

    // Método para obter o próximo ID disponível
    private static int GetNextTableID()
    {
        // Verifica se há IDs excluídos que podem ser reutilizados
        if (deletedTableIDs.Count > 0)
        {
            // Reutiliza o menor ID disponível
            int availableID = deletedTableIDs.Min();
            deletedTableIDs.Remove(availableID); // Remove o ID da lista de IDs excluídos
            return availableID;
        }
        else
        {
            // Se não houver IDs excluídos, usa o próximo ID sequencial
            return nextTableID++;
        }
    }

    
    void Start()
    {

        // Gerar um ID sequencial para a tabela
        tableID = GetNextTableID(); // Usa o novo método para obter o ID
        tableGenerator.SetTableID(tableID);

        // Assinar eventos locais
        tableGenerator.OnTableGenerated += PopulateSlots;
        GenerateTable();

        // Passa o tableID para todas as DropZones
        SetTableIDForDropZones();

        // Assinar o evento OnDrop apenas para esta tabela
        DragAndDrop2D.OnDrop += OnObjectDropped;
        
    }


    private void SetTableIDForDropZones()
    {
        // Encontre todos os CircularDropZones na cena ou dentro de um painel específico
        CircularDropZone[] dropZones = FindObjectsOfType<CircularDropZone>();
        
        foreach (var dropZone in dropZones)
        {
            dropZone.SetTableID(tableID);
        }
    }
    private void OnDestroy()
    {
        // Adiciona o ID da tabela excluída na lista
        deletedTableIDs.Add(tableID);
        // Cancelar inscrição para evitar referências a objetos destruídos
        DragAndDrop2D.OnDrop -= OnObjectDropped;
        RestoreQuantum();
        

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
        if (objectsAlreadyAdded.Contains(droppedObject)) return;

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
            // Verifica se é um CircularDropZone e pertence à tabela correta
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
        // Calcula quantas colunas o objeto vai ocupar
        int columnsToOccupy = Mathf.Min(Quantum, Mathf.CeilToInt(executionTime));

        // Reduz o tempoExecucao do objeto e registra o quantum subtraído
        PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
        if (objectData != null)
        {
            int previousTempo = objectData.tempoExecucao;

            // Subtração com limite mínimo de zero
            int subtracted = Mathf.Min(Quantum, previousTempo); // Subtração sem passar do limite
            objectData.tempoExecucao = Mathf.Max(0, previousTempo - Quantum);

            // Registra o quantum realmente subtraído
            if (!quantumSubtracted.ContainsKey(obj))
            {
                quantumSubtracted[obj] = 0;
            }
            quantumSubtracted[obj] += subtracted;

            // Acumula o valor subtraído no tempoExecucaoTotal
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


    public void RestoreQuantum()
    {
        foreach (var entry in quantumSubtracted)
        {
            GameObject obj = entry.Key;
            int quantumToRestore = entry.Value;

            PuzzleObjectData objectData = obj.GetComponent<PuzzleObjectData>();
            if (objectData != null)
            {
                // Restaurar o tempoExecucao original
                objectData.tempoExecucao += quantumToRestore;
                Debug.Log($"Quantum {quantumToRestore} restaurado para o objeto {objectData.name}.");

                // Restaurar o tempoExecucaoTotal também
                objectData.tempoExecucaoTotal -= quantumToRestore;
                Debug.Log($"Tempo total executado restaurado para {objectData.name}: {objectData.tempoExecucaoTotal}");
            }
        }

        // Limpa o registro após a restauração
        quantumSubtracted.Clear();
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
