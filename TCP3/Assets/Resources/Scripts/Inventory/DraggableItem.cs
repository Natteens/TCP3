using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int originalItemIndex;
    public Image image;
    private Color originalColor;
    public float dragAlpha = 0.5f;
    public Transform ItemContainer;
    private UI_Inventory uiInventory;

    public void Start()
    {
        ItemContainer = GameObject.Find("ItemSlotContainer").transform;
        uiInventory = FindObjectOfType<UI_Inventory>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            parentAfterDrag = transform.parent;
            originalItemIndex = parentAfterDrag.GetSiblingIndex(); // Guarda a posição original na lista de itens
            transform.SetParent(ItemContainer);
            transform.SetAsLastSibling();

            if (image != null)
            {
                originalColor = image.color;
                Color color = image.color;
                color.a = dragAlpha;
                image.color = color;
                image.raycastTarget = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;

            if (image != null)
            {
                image.color = originalColor;
                image.raycastTarget = true;
            }

            // Atualizar a lista de itens no inventário
            int newSlotIndex = parentAfterDrag.GetSiblingIndex();
            uiInventory.UpdateItemPosition(originalItemIndex, newSlotIndex);
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
