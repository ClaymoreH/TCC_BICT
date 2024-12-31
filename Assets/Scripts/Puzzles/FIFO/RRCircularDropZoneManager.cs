using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necessário para usar o TMP_Dropdown
using UnityEngine.UI;

public class CircularDropZoneManager : MonoBehaviour
{
    public TMP_Dropdown dropdownMenu;
    public List<GameObject> circularDropZones;  // A lista já é pública, então você pode acessá-la diretamente
    public List<GameObject> tables;
    public GameObject dropZonePrefab;
    public GameObject tablePrefab;
    public Transform parentPanel;
    public Transform parentTablePanel;
    public Button addButton;
    public Button removeButton;

    // Método de acesso à lista de DropZones
    public List<GameObject> GetCircularDropZones()
    {
        return circularDropZones;
    }

    private void Start()
    {
        if (circularDropZones.Count == 0)
        {
            AddDropZone();
        }
        dropdownMenu.onValueChanged.AddListener(OnDropdownValueChanged);
        addButton.onClick.AddListener(AddDropZone);
        removeButton.onClick.AddListener(RemoveDropZone);

        UpdateDropZoneVisibility(dropdownMenu.value);
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        UpdateDropZoneVisibility(selectedIndex);
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

        PositionTables();
        UpdateDropdownOptions();
        dropdownMenu.value = circularDropZones.Count - 1;

        // Garantir que a nova drop zone seja visível
        UpdateDropZoneVisibility(dropdownMenu.value);
    }


    private void RemoveDropZone()
    {
        int lastIndex = circularDropZones.Count - 1;
        Destroy(circularDropZones[lastIndex]);
        Destroy(tables[lastIndex]);
        circularDropZones.RemoveAt(lastIndex);
        tables.RemoveAt(lastIndex);

        PositionTables();
        UpdateDropdownOptions();
        dropdownMenu.value = circularDropZones.Count - 1;
        UpdateDropZoneVisibility(dropdownMenu.value);

    }

    private void UpdateDropdownOptions()
    {
        dropdownMenu.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < circularDropZones.Count; i++)
        {
            options.Add("CIRCLE " + (i + 1));
        }
        dropdownMenu.AddOptions(options);
    }

    private void PositionTables()
    {
        float spacing = 200f;
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