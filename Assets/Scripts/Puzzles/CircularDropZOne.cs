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

    private Color[] originalColors; // To store the original colors of the slices

    private void Start()
    {
        // Store the original colors of each slice at the start
        originalColors = new Color[imagesPieChart.Length];
        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            if (imagesPieChart[i] != null)
            {
                originalColors[i] = imagesPieChart[i].color;
                imagesPieChart[i].color = initialColor; // Set the initial color
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && transform.childCount < maxChildren)
        {
            DragAndDrop2D draggedObject = eventData.pointerDrag.GetComponent<DragAndDrop2D>();

            if (draggedObject != null)
            {
                PlaceObject(draggedObject);
                UpdatePieChart();
            }
        }
    }

    private void PlaceObject(DragAndDrop2D draggedObject)
    {
        draggedObject.transform.SetParent(transform);
        RectTransform draggedRectTransform = draggedObject.GetComponent<RectTransform>();

        int childIndex = transform.childCount - 1;
        float angle = (360f / maxChildren) * childIndex;

        Vector3 positionOffset = Quaternion.Euler(0f, 0f, angle) * Vector3.up * radius;
        draggedRectTransform.anchoredPosition = positionOffset;

        // Reset anchors and pivot for proper positioning
        draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);

        draggedRectTransform.localScale = Vector3.one;
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
                imagesPieChart[i].fillAmount = cumulativeFill + sliceSize;
                imagesPieChart[i].color = originalColors[i]; // Restore the original color
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
