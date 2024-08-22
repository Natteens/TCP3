using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (draggedItem != null)
        {
            // Verifica se o slot de destino já tem um item
            if (transform.childCount > 0)
            {
                DraggableItem existingItem = transform.GetChild(0).GetComponent<DraggableItem>();

                // Troca os itens entre os slots
                Transform originalParent = draggedItem.parentAfterDrag;
                draggedItem.transform.SetParent(transform);
                draggedItem.transform.localPosition = Vector3.zero;

                existingItem.transform.SetParent(originalParent);
                existingItem.transform.localPosition = Vector3.zero;

                // Atualiza as referências dos pais
                draggedItem.parentAfterDrag = transform;
                existingItem.parentAfterDrag = originalParent;
            }
            else
            {
                // Se o slot de destino estiver vazio, move o item para o novo slot
                draggedItem.parentAfterDrag = transform;
                draggedItem.transform.SetParent(transform); // Define o novo parent imediatamente
                draggedItem.transform.localPosition = Vector3.zero; // Reseta a posição local
            }
        }
    }
}
