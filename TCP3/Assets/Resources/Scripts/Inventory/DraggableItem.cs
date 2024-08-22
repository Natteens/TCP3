using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    private Color originalColor;
    public float dragAlpha = 0.5f;
    public Transform ItemContainer;

    public void Start()
    {
        ItemContainer = GameObject.Find("ItemSlotContainer").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            parentAfterDrag = transform.parent;
            transform.SetParent(ItemContainer);
            transform.SetAsLastSibling(); 

            if (image != null)
            {
                originalColor = image.color; // Guarda a cor original
                Color color = image.color;
                color.a = dragAlpha; // Define a nova transparência
                image.color = color;
                image.raycastTarget = false;
            }

            // Ajustar a posição para que o item arrastado fique visível
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = eventData.position / rectTransform.lossyScale.x;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = Input.mousePosition; // Move o item arrastado com o mouse
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        transform.SetParent(parentAfterDrag);
      

        transform.localPosition = Vector3.zero;

        if (image != null)
        {
            image.color = originalColor; // Restaura a cor original
            image.raycastTarget = true;
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;

        Image img = GetComponent<Image>();
        TextMeshProUGUI txt = img.GetComponentInChildren<TextMeshProUGUI>();

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
