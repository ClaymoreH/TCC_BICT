using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public bool acceptAnyObject = true; 
    public List<string> allowedTags;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Verifica se o objeto arrastado é válido
        if (eventData.pointerDrag != null)
        {
            DragAndDrop2D draggedObject = eventData.pointerDrag.GetComponent<DragAndDrop2D>();

            if (draggedObject != null && IsObjectAllowed(draggedObject.gameObject))
            {
                // Verifica se já há um objeto neste slot
                Transform existingObject = transform.childCount > 0 ? transform.GetChild(0) : null;

                if (existingObject != null)
                {
                    // Troca de posição com o objeto arrastado
                    SwapObjects(draggedObject, existingObject.GetComponent<DragAndDrop2D>());
                }
                else
                {
                    // Se o slot estiver vazio, coloca o objeto nele
                    PlaceObject(draggedObject);
                }
            }
        }
    }

    private void SwapObjects(DragAndDrop2D draggedObject, DragAndDrop2D existingObject)
    {
        // Salva o pai original e a posição do objeto existente
        Transform draggedParent = draggedObject.transform.parent;
        Vector2 draggedPosition = draggedObject.GetComponent<RectTransform>().anchoredPosition;

        // Move o objeto existente para o lugar do arrastado
        existingObject.transform.SetParent(draggedParent);
        existingObject.GetComponent<RectTransform>().anchoredPosition = draggedPosition;

        // Coloca o objeto arrastado no slot atual
        PlaceObject(draggedObject);
    }

    private void PlaceObject(DragAndDrop2D draggedObject)
    {
        // Torna o objeto arrastado filho da área de drop
        RectTransform draggedRect = draggedObject.GetComponent<RectTransform>();
        draggedRect.SetParent(rectTransform);

        // Centraliza o objeto na área
        draggedRect.anchoredPosition = Vector2.zero;
        draggedRect.localScale = Vector3.one; // Mantém a escala
    }

    private bool IsObjectAllowed(GameObject obj)
    {
        // Verifica se o objeto tem uma tag permitida ou se aceita qualquer objeto
        return acceptAnyObject || (allowedTags != null && allowedTags.Contains(obj.tag));
    }
}
