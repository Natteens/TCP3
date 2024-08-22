using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        DraggableItem thisSlotItem = GetComponentInChildren<DraggableItem>();

        if (draggedItem != null)
        {
            if (thisSlotItem != null && thisSlotItem != draggedItem)
            {
                Item tempItem = thisSlotItem.item;
                thisSlotItem.SetItem(draggedItem.item);
                draggedItem.SetItem(tempItem);
            }
            else if (thisSlotItem == null)
            {
                draggedItem.transform.SetParent(transform, false);
                draggedItem.rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
