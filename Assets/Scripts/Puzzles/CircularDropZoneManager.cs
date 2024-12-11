using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necessário para usar o TMP_Dropdown
using UnityEngine.UI;

public class CircularDropZoneManager : MonoBehaviour
{
    public TMP_Dropdown dropdownMenu; // Referência ao TMP_Dropdown
    public List<GameObject> circularDropZones; // Lista de DropZones circulares
    public GameObject dropZonePrefab; // Prefab da DropZone que será instanciado
    public Transform parentPanel; // Painel pai onde as DropZones serão organizadas

    public Button addButton; // Botão para adicionar DropZone
    public Button removeButton; // Botão para remover DropZone

    private void Start()
    {
        // Listener para mudanças no dropdown
        dropdownMenu.onValueChanged.AddListener(OnDropdownValueChanged);

        // Listeners para botões
        addButton.onClick.AddListener(AddDropZone);
        removeButton.onClick.AddListener(RemoveDropZone);

        // Configura a visibilidade inicial
        UpdateDropZoneVisibility(dropdownMenu.value);
        UpdateRemoveButtonState();
    }

    private void OnDropdownValueChanged(int selectedIndex)
    {
        // Atualiza a visibilidade das DropZones com base na seleção
        UpdateDropZoneVisibility(selectedIndex);
        UpdateRemoveButtonState(); // Atualiza o estado do botão de excluir
    }

    private void UpdateDropZoneVisibility(int selectedIndex)
    {
        for (int i = 0; i < circularDropZones.Count; i++)
        {
            if (circularDropZones[i] != null)
            {
                circularDropZones[i].SetActive(i == selectedIndex);
            }
        }
    }

    private void AddDropZone()
    {
        // Instancia uma nova DropZone dentro do painel pai
        GameObject newDropZone = Instantiate(dropZonePrefab, parentPanel);

        // Define o mesmo posicionamento e escala que a original
        newDropZone.transform.localPosition = Vector3.zero;
        newDropZone.transform.localScale = Vector3.one;

        // Adiciona à lista
        circularDropZones.Add(newDropZone);

        // Atualiza o dropdown
        UpdateDropdownOptions();

        // Define o dropdown para a nova DropZone criada
        dropdownMenu.value = circularDropZones.Count - 1; // Última DropZone

        // Atualiza o estado do botão de excluir
        UpdateRemoveButtonState();
    }

    private void RemoveDropZone()
    {
        if (circularDropZones.Count > 1) // Permitir excluir apenas se houver mais de uma zona
        {
            // Remove a última DropZone
            int lastIndex = circularDropZones.Count - 1;
            GameObject lastDropZone = circularDropZones[lastIndex];
            circularDropZones.RemoveAt(lastIndex);

            // Destroi o GameObject
            Destroy(lastDropZone);

            // Atualiza o dropdown
            UpdateDropdownOptions();

            // Define o dropdown para a última DropZone remanescente
            dropdownMenu.value = circularDropZones.Count - 1;
            UpdateDropZoneVisibility(dropdownMenu.value);

            // Atualiza o estado do botão de excluir
            UpdateRemoveButtonState();
        }
    }



    private void UpdateDropdownOptions()
    {
        // Limpa as opções antigas
        dropdownMenu.ClearOptions();

        // Cria novas opções com base na quantidade de DropZones
        List<string> options = new List<string>();
        for (int i = 0; i < circularDropZones.Count; i++)
        {
            options.Add("Opção " + (i + 1));
        }

        // Adiciona as opções no dropdown
        dropdownMenu.AddOptions(options);
    }

    private void UpdateRemoveButtonState()
    {
        // O botão de excluir só estará ativo se houver mais de uma DropZone
        removeButton.interactable = circularDropZones.Count > 1 && dropdownMenu.value == circularDropZones.Count - 1;
    }

}
