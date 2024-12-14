using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necessário para usar o TMP_Dropdown
using UnityEngine.UI;

public class CircularDropZoneManager : MonoBehaviour
{
    public TMP_Dropdown dropdownMenu;
    public List<GameObject> circularDropZones;
    public List<GameObject> tables; // Lista das tabelas associadas
    public GameObject dropZonePrefab;
    public GameObject tablePrefab;  // Prefab da tabela
    public Transform parentPanel;
    public Transform parentTablePanel; // Painel que conterá as tabelas lado a lado

    public Button addButton;
    public Button removeButton;

    private void Start()
    {
        dropdownMenu.onValueChanged.AddListener(OnDropdownValueChanged);
        addButton.onClick.AddListener(AddDropZone);
        removeButton.onClick.AddListener(RemoveDropZone);

        UpdateDropZoneVisibility(dropdownMenu.value);
        UpdateRemoveButtonState();
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        UpdateDropZoneVisibility(selectedIndex);
        UpdateRemoveButtonState();
    }

    private void UpdateDropZoneVisibility(int selectedIndex)
    {
        for (int i = 0; i < circularDropZones.Count; i++)
        {
            circularDropZones[i].SetActive(i == selectedIndex);
        }
    }

    private void AddDropZone()
    {
        // Instancia a DropZone
        GameObject newDropZone = Instantiate(dropZonePrefab, parentPanel);
        newDropZone.transform.localPosition = Vector3.zero;
        newDropZone.transform.localScale = Vector3.one;
        circularDropZones.Add(newDropZone);

        // Instancia a Tabela correspondente
        GameObject newTable = Instantiate(tablePrefab, parentTablePanel);
        newTable.transform.localPosition = Vector3.zero;
        newTable.transform.localScale = Vector3.one;
        tables.Add(newTable);

        PositionTables(); // Organiza as tabelas lado a lado

        UpdateDropdownOptions();
        dropdownMenu.value = circularDropZones.Count - 1;

        UpdateRemoveButtonState();
    }

    private void RemoveDropZone()
    {
        if (circularDropZones.Count > 1)
        {
            int lastIndex = circularDropZones.Count - 1;

            Destroy(circularDropZones[lastIndex]);
            Destroy(tables[lastIndex]); // Remove também a tabela correspondente

            circularDropZones.RemoveAt(lastIndex);
            tables.RemoveAt(lastIndex);

            PositionTables(); // Atualiza a posição das tabelas

            UpdateDropdownOptions();
            dropdownMenu.value = circularDropZones.Count - 1;

            UpdateDropZoneVisibility(dropdownMenu.value);
            UpdateRemoveButtonState();
        }
    }

    private void UpdateDropdownOptions()
    {
        dropdownMenu.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < circularDropZones.Count; i++)
        {
            options.Add("Opção " + (i + 1));
        }
        dropdownMenu.AddOptions(options);
    }

    private void UpdateRemoveButtonState()
    {
        removeButton.interactable = circularDropZones.Count > 1;
    }

    private void PositionTables()
    {
        float spacing = 200f; // Espaçamento horizontal entre as tabelas
        for (int i = 0; i < tables.Count; i++)
        {
            RectTransform rectTransform = tables[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(i * spacing, 0);
            }
        }
    }
}
