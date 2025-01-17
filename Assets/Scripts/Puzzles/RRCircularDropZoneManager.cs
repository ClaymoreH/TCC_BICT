using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class CircularDropZoneManager : MonoBehaviour
{
    public TMP_Dropdown dropdownMenu;
    public List<GameObject> circularDropZones; 
    public List<GameObject> tables;
    public GameObject dropZonePrefab;
    public GameObject tablePrefab;
    public Transform parentPanel;
    public Transform parentTablePanel;
    public Button addButton;
    public Button removeButton;

    public List<GameObject> GetCircularDropZones()
    {
        return circularDropZones;
    }

    private void Start()
    {
        // Encontra automaticamente o RRSlotManager na cena
        RRSlotManager rrSlotManager = FindObjectOfType<RRSlotManager>();

        if (circularDropZones.Count == 0)
        {
            AddDropZone();
        }
        dropdownMenu.onValueChanged.AddListener(OnDropdownValueChanged);
        addButton.onClick.AddListener(AddDropZone);
        removeButton.onClick.AddListener(() => RemoveDropZone(rrSlotManager));

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

        // Busca o componente CircularDropZone no objeto filho "Ciclo"
        Transform ciclo = newDropZone.transform.Find("Ciclo");
        if (ciclo != null)
        {
            CircularDropZone dropZoneScript = ciclo.GetComponent<CircularDropZone>();
            if (dropZoneScript != null)
            {
                dropZoneScript.dropzoneID = circularDropZones.Count;  // Atribuindo o dropzoneID
                Debug.Log("DropZone ID atribuído: " + dropZoneScript.dropzoneID);
            }
            else
            {
                Debug.LogError("CircularDropZone não encontrado no objeto 'Ciclo'!");
            }
        }
        else
        {
            Debug.LogError("Objeto 'Ciclo' não encontrado no novo GameObject!");
        }

        // Adiciona a DropZone e a Tabela às listas
        circularDropZones.Add(newDropZone);

        // Atualiza a posição das tabelas e o dropdown
        UpdateDropdownOptions();
        dropdownMenu.value = circularDropZones.Count - 1;

        // Garantir que a nova drop zone seja visível
        UpdateDropZoneVisibility(dropdownMenu.value);
    }

 
    private void RemoveDropZone(RRSlotManager rrSlotManager)
    {
        int lastIndex = circularDropZones.Count - 1;
        
        // Chama o método ClearTable em vez de destruir a tabela diretamente
        int dropzoneIDToClear = circularDropZones[lastIndex].GetComponentInChildren<CircularDropZone>().dropzoneID;
        if (rrSlotManager != null)
        {
            rrSlotManager.ClearTable(dropzoneIDToClear);  // Chama ClearTable para limpar a tabela
        }
        else
        {
            Debug.LogError("RRSlotManager não encontrado!");
        }

        // Agora remove apenas a DropZone (sem destruir a tabela)
        Destroy(circularDropZones[lastIndex]);

        // Remove a DropZone da lista
        circularDropZones.RemoveAt(lastIndex);
        
        // Atualiza a posição das tabelas e o dropdown
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
            options.Add("   CICLO " + (i + 1));
        }
        dropdownMenu.AddOptions(options);
    }
}