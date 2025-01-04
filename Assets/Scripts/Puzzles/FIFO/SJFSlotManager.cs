using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotManager : MonoBehaviour
{
    public int tableID; 
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    private int currentRow = 1;
    private int currentColumn = 1;
    private bool isTableFull = false;
    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();

    public Transform parentOriginal;
    private Dictionary<int, int> lastColumnInRow = new Dictionary<int, int>(); 
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

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
            SlotIdentifier identifier = child.GetComponent<SlotIdentifier>();
            if (identifier != null && identifier.tableID == tableID) 
            {
                slots.Add(child.gameObject);
            }
        }
    }

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
            if (slotIdentifier != null && slotIdentifier.tableID == tableID)
            {
                AddObjectToTable(droppedObject);
                return;
            }
        }
    }

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

    private void PlaceObjectInSlot(GameObject obj, GameObject slot)
    {
        // Configura o objeto para ser filho do slot
        obj.transform.SetParent(slot.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.GetComponent<RectTransform>().localScale = Vector3.one;

        // Obtém as referências aos componentes de imagem
        Image slotImage = slot.GetComponent<Image>();
        Image objImage = obj.GetComponent<Image>();

        if (slotImage != null && objImage != null)
        {
            // Aplica a cor do objeto ao slot
            slotImage.color = objImage.color;

            // Torna o objeto original invisível, mas mantém seus dados
            objImage.enabled = false;
        }
    }
    private void MoveToNextPosition()
    {
        currentColumn++;
        if (currentColumn > tableGenerator.columns)
        {
            currentColumn = 1;
            currentRow++;
        }
    }

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

    private GameObject FindNextAvailableSlot()
    {
        return FindSlot(currentRow, currentColumn);
    }

    public void ResetTable()
    {
        foreach (GameObject slot in slots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                Image childImage = child.GetComponent<Image>(); // Obtém a referência ao componente Image

                // Verifica se o objeto tem uma referência à posição original
                if (originalPositions.ContainsKey(child.gameObject))
                {
                    // Restaura o objeto para sua posição original
                    child.SetParent(parentOriginal);
                    child.position = originalPositions[child.gameObject];

                    // Torna o objeto visível novamente, caso tenha sido ocultado
                    if (childImage != null)
                    {
                        childImage.enabled = true;
                    }
                }
                else
                {
                    // Se não tem posição original, destrói o objeto
                    Destroy(child.gameObject);
                }
            }

            // Restaura a transparência dos slots (fazendo com que fiquem invisíveis)
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                Color currentColor = slotImage.color;
                slotImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f); // Define alpha para 0
            }
        }

        // Limpa o estado da tabela
        objectsAlreadyAdded.Clear();
        originalPositions.Clear();
        currentRow = 1;
        currentColumn = 1;
        isTableFull = false;
        lastColumnInRow.Clear();

        Debug.Log($"Tabela {tableID} resetada e pronta para reutilização.");
    }




}
