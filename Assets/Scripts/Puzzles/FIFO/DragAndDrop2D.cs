using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 initialPosition;
    private Transform initialParent;

    public bool returnToOriginalPosition = true; 

    [Header("Configuração de Som")]
    public AudioSource audioSource;
    public AudioClip dropSound;

    public delegate void DropEventHandler(GameObject droppedObject);
    public static event DropEventHandler OnDrop;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Salva a posição inicial e o pai inicial
        initialPosition = rectTransform.anchoredPosition;
        initialParent = transform.parent;

        // Configura o Canvas Group para permitir arrastar REFAZER 
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move com o mouse
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reseta o Canvas Group
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Se nãosolto em uma área válida, retorna à posição inicial
        if (transform.parent == initialParent && returnToOriginalPosition)
        {
            rectTransform.anchoredPosition = initialPosition;
        }

        // Dispara o drop para o controlador
        OnDrop?.Invoke(gameObject);

        TocarSomDeDrop();
    }

    private void TocarSomDeDrop()
    {
        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
    }
}
