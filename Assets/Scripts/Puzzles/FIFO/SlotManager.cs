using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public DynamicTableGenerator tableGenerator;
    private int currentRow = 1;
    private int currentColumn = 1;
    private bool isTableFull = false;
    private HashSet<GameObject> objectsAlreadyAdded = new HashSet<GameObject>();

    public Transform parentOriginal; // Novo campo: referência ao pai original dos objetos

    // Armazena a última coluna utilizada em cada linha
    private Dictionary<int, int> lastColumnInRow = new Dictionary<int, int>();

    // Armazena as posições originais dos objetos
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        // Registra os eventos e cria a tabela
        DragAndDrop2D.OnDrop += OnObjectDropped;
        tableGenerator.OnTableGenerated += PopulateSlots;
        tableGenerator.GenerateTable(tableGenerator.rows, tableGenerator.columns);
    }

    // Popula a lista de slots com os slots da tabela gerada
    private void PopulateSlots()
    {
        slots.Clear();
        foreach (Transform child in tableGenerator.parentPanel)
        {
            slots.Add(child.gameObject);
        }
    }

    // Método chamado quando um objeto é solto
    private void OnObjectDropped(GameObject droppedObject)
    {
        if (isTableFull || objectsAlreadyAdded.Contains(droppedObject)) return;

        AddObjectToTable(droppedObject);
    }

    // Adiciona o objeto à tabela e gerencia sua posição
    private void AddObjectToTable(GameObject droppedObject)
    {
        float executionTime = droppedObject.GetComponent<PuzzleObjectData>()?.tempoExecucao ?? 1;

        // Armazena a posição original do objeto
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

                // Verifica se a tabela está cheia
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

        // Atualiza a última coluna ocupada da linha
        if (!lastColumnInRow.ContainsKey(currentRow))
        {
            lastColumnInRow[currentRow] = currentColumn - 1;
        }

        // Atualiza a linha e a coluna para o próximo objeto
        currentRow++;
        currentColumn = lastColumnInRow.ContainsKey(currentRow - 1) ? lastColumnInRow[currentRow - 1] + 1 : 1;
    }

    // Coloca o objeto dentro de um slot
    private void PlaceObjectInSlot(GameObject obj, GameObject slot)
    {
        obj.transform.SetParent(slot.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    // Atualiza a posição atual na tabela para o próximo slot disponível
    private void MoveToNextPosition()
    {
        currentColumn++;

        if (currentColumn > tableGenerator.columns)
        {
            currentColumn = 1;
            currentRow++;
        }
    }

    // Encontra um slot específico de acordo com a linha e coluna
    public GameObject FindSlot(int row, int column)
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

    // Encontra o próximo slot disponível
    private GameObject FindNextAvailableSlot()
    {
        return FindSlot(currentRow, currentColumn);
    }

    // Novo método: Reseta a tabela e move os objetos de volta para o pai de origem
    public void ResetTable()
    {
        foreach (GameObject slot in slots)
        {
            // Checa se o slot contém objetos
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0); // Obtém o objeto filho

                // Verifica se o objeto é o original, não o clone
                if (originalPositions.ContainsKey(child.gameObject))
                {
                    // Envia o objeto original de volta para o pai original
                    child.SetParent(parentOriginal);
                    child.position = originalPositions[child.gameObject]; // Reseta a posição para a original
                }
                else
                {
                    // Se for um clone, destrói-o
                    Destroy(child.gameObject);
                }
            }
        }

        // Limpa os dados
        objectsAlreadyAdded.Clear(); // Limpa a lista de objetos adicionados
        originalPositions.Clear();  // Limpa as posições originais

        // Reseta os indicadores de posição
        currentRow = 1; 
        currentColumn = 1; 
        isTableFull = false;

        // Reinicia os dados de preenchimento de linhas
        lastColumnInRow.Clear();

        Debug.Log("Tabela resetada e pronta para reutilização.");
    }

}
