using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 originalPosition;
    private Image draggedImage;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;

            // Criar uma imagem fantasma para seguir o mouse
            draggedImage = new GameObject("DraggedImage").AddComponent<Image>();
            draggedImage.transform.SetParent(canvas.transform);
            draggedImage.sprite = gameObject.transform.Find("image").GetComponent<Image>().sprite;
            draggedImage.raycastTarget = false;
            draggedImage.rectTransform.sizeDelta = rectTransform.sizeDelta;

            canvasGroup.alpha = 0.6f; // Reduzir a opacidade do item original para indicar que está sendo arrastado
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null && draggedImage != null)
        {
            draggedImage.rectTransform.position = Input.mousePosition; // Fazer a imagem fantasma seguir o mouse
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedImage != null)
        {
            Destroy(draggedImage.gameObject); // Destruir a imagem fantasma
        }

        canvasGroup.alpha = 1f; // Restaurar a opacidade do item original

        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("InventorySlot"))
        {
            rectTransform.anchoredPosition = originalPosition; // Retornar à posição original se não for solto em um slot válido
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;

        Image img = GetComponent<Image>();
        TextMeshProUGUI txt = transform.Find("amount").GetComponent<TextMeshProUGUI>();

        if (img != null && txt != null)
        {
            if (item != null)
            {
                img.sprite = item.itemSprite;
                txt.text = item.amount.ToString();
            }
            else
            {
                img.sprite = null;
                txt.text = "";
            }
        }
    }
}
