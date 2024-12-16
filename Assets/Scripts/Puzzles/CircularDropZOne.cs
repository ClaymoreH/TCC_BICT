using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CircularDropZone : MonoBehaviour, IDropHandler
{
    public int maxChildren = 4; // Maximum number of objects allowed
    public float radius = 50f; // Radius of the circle

    public Image[] imagesPieChart; // Array to define the colors of the pie chart slices
    public Color initialColor = Color.gray; // Initial color when the chart is empty
    public float holeRadius = 20f; // The radius of the hole in the center (open space)

    public GameObject associatedTable; // Referência à tabela associada
    public int tableID; // ID da tabela associada a este DropZone

    private void Start()
    {
        // Set initial color for all slices
        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            if (imagesPieChart[i] != null)
            {
                imagesPieChart[i].color = initialColor; // Set the initial color
            }
        }

    }
    
    public void SetTableID(int id)
    {
        tableID = id;
    }

    public void SetAssociatedTable(GameObject table)
    {
        associatedTable = table;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && transform.childCount < maxChildren)
        {
            DragAndDrop2D draggedObject = eventData.pointerDrag.GetComponent<DragAndDrop2D>();

            if (draggedObject != null)
            {
                // Verifica se já existe um objeto com o mesmo valor de processo
                PuzzleObjectData objectData = draggedObject.gameObject.GetComponent<PuzzleObjectData>();
                if (objectData == null) return;

                // Verifica se já existe um objeto com o mesmo processo na DropZone
                if (IsProcessAlreadyAssigned(objectData.processo))
                {
                    Debug.Log("Objeto com esse processo já foi adicionado à DropZone.");
                    return; // Impede o drop se o processo já existir
                }

                // Clona o objeto arrastado e coloca-o na zona
                GameObject clonedObject = Instantiate(draggedObject.gameObject, transform);
                clonedObject.transform.SetParent(transform);

                // Ajusta o clone
                RectTransform clonedRectTransform = clonedObject.GetComponent<RectTransform>();
                clonedRectTransform.localScale = Vector3.one;

                // Posiciona o clone na zona circular
                PlaceObject(clonedRectTransform);

                // Atualiza o gráfico de pizza com a cor do objeto
                UpdatePieChart();
            }
        }
    }

    private bool IsProcessAlreadyAssigned(int processo)
    {
        // Verifica se algum dos filhos da DropZone tem o mesmo valor de processo
        foreach (Transform child in transform)
        {
            PuzzleObjectData childData = child.GetComponent<PuzzleObjectData>();
            if (childData != null && childData.processo == processo)
            {
                return true; // Já existe um objeto com esse processo
            }
        }
        return false;
    }

    private void PlaceObject(RectTransform draggedRectTransform)
    {
        int childIndex = transform.childCount - 1;
        float angle = (360f / maxChildren) * childIndex;

        Vector3 positionOffset = Quaternion.Euler(0f, 0f, angle) * Vector3.up * radius;
        draggedRectTransform.anchoredPosition = positionOffset;

        // Reset anchors e pivot para o posicionamento correto
        draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private void UpdatePieChart()
    {
        int currentChildren = transform.childCount;

        if (currentChildren == 0 || imagesPieChart.Length == 0)
        {
            // Reset all slices to the initial color when there are no children
            for (int i = 0; i < imagesPieChart.Length; i++)
            {
                imagesPieChart[i].fillAmount = 0;
                imagesPieChart[i].color = initialColor;
                imagesPieChart[i].gameObject.SetActive(true); // Ensure they are visible
            }
            return;
        }

        float sliceSize = 1f / currentChildren; // Each slice's proportional size
        float cumulativeFill = 0;

        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            if (i < currentChildren)
            {
                // Obtém o objeto correspondente
                Transform child = transform.GetChild(i);
                Image childImage = child.GetComponent<Image>();
                
                if (childImage != null)
                {
                    imagesPieChart[i].color = childImage.color; // Define a cor com base no objeto
                }

                imagesPieChart[i].fillAmount = cumulativeFill + sliceSize;
                cumulativeFill += sliceSize;

                // Enable the image to make it visible
                imagesPieChart[i].gameObject.SetActive(true);
            }
            else
            {
                // Hide the extra slices if they are not used
                imagesPieChart[i].fillAmount = 0;
                imagesPieChart[i].color = initialColor; // Reset to initial color
                imagesPieChart[i].gameObject.SetActive(false);
            }
        }

        // Apply the hole effect by reducing the central area
        ApplyHoleEffect();
    }

    private void ApplyHoleEffect()
    {
        foreach (var image in imagesPieChart)
        {
            // We are making the middle part transparent, the 'hole'
            // Adjust the radial fill so that the "center" is open
            image.material.SetFloat("_FillCenter", holeRadius / radius);
        }
    }
}
