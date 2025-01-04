using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CircularDropZone : MonoBehaviour, IDropHandler
{
    public int maxChildren = 4; 
    public float radius = 50f;

    public Image[] imagesPieChart;
    public Color initialColor = Color.gray;
    public float holeRadius = 20f;

    public GameObject associatedTable;
    public int tableID; 
    public int dropzoneID;

    private void Start()
    {
        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            if (imagesPieChart[i] != null)
            {
                imagesPieChart[i].color = initialColor;
            }
        }

    }

    public void SetTableID(int id)
    {
        tableID = 0;
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
                PuzzleObjectData objectData = draggedObject.gameObject.GetComponent<PuzzleObjectData>();
                if (objectData == null) return;
                objectData.dropzoneID = dropzoneID; // Atribui o ID da DropZone ao objeto

                if (objectData.tempoExecucao <= 0)
                {
                    Debug.Log("O objeto não pode ser adicionado à DropZone pois o tempoExecucao é 0.");
                    return;
                }

                if (IsProcessAlreadyAssigned(objectData.processo))
                {
                    Debug.Log("Objeto com esse processo já foi adicionado à DropZone.");
                    return;
                }

                GameObject clonedObject = Instantiate(draggedObject.gameObject, transform);
                clonedObject.transform.SetParent(transform);

                RectTransform clonedRectTransform = clonedObject.GetComponent<RectTransform>();
                clonedRectTransform.localScale = Vector3.one;

                PlaceObject(clonedRectTransform);

                UpdatePieChart();
            }
        }
    }

    private bool IsProcessAlreadyAssigned(int processo)
    {
        foreach (Transform child in transform)
        {
            PuzzleObjectData childData = child.GetComponent<PuzzleObjectData>();
            if (childData != null && childData.processo == processo)
            {
                return true;
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

        draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private void UpdatePieChart()
    {
        int currentChildren = transform.childCount;

        if (currentChildren == 0 || imagesPieChart.Length == 0)
        {
            for (int i = 0; i < imagesPieChart.Length; i++)
            {
                imagesPieChart[i].fillAmount = 0;
                imagesPieChart[i].color = initialColor;
                imagesPieChart[i].gameObject.SetActive(true);
            }
            return;
        }

        float sliceSize = 1f / currentChildren;
        float cumulativeFill = 0;

        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            if (i < currentChildren)
            {
                Transform child = transform.GetChild(i);
                Image childImage = child.GetComponent<Image>();
                
                if (childImage != null)
                {
                    imagesPieChart[i].color = childImage.color; 
                }

                imagesPieChart[i].fillAmount = cumulativeFill + sliceSize;
                cumulativeFill += sliceSize;

                imagesPieChart[i].gameObject.SetActive(true);
            }
            else
            {
                imagesPieChart[i].fillAmount = 0;
                imagesPieChart[i].color = initialColor;
                imagesPieChart[i].gameObject.SetActive(false);
            }
        }

        ApplyHoleEffect();
    }

    private void ApplyHoleEffect()
    {
        foreach (var image in imagesPieChart)
        {
            image.material.SetFloat("_FillCenter", holeRadius / radius);
        }
    }
}
