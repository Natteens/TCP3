using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public event EventHandler OnItemListChanged;
    [SerializeField] private List<Item> itemList;
    [SerializeField] private int maxSlots = 30;
    [SerializeField] private int slotsWithItem = 0;

    public Inventory()
    {
        itemList = new List<Item>();
    }

    public void AddItem(Item item)
    {
        if (!CanPickup()) return;

        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList)
            {
                // Verifica se o item no inventário é o mesmo (não apenas o tipo, mas o próprio item)
                if (inventoryItem.itemType == item.itemType
                    && inventoryItem.itemName == item.itemName
                    && inventoryItem.itemSprite == item.itemSprite
                   )
                {
                    inventoryItem.amount += 1;
                    itemAlreadyInInventory = true;
                    break;
                }
            }

            if (!itemAlreadyInInventory)
            {
                item.amount = 1; // Define a quantidade para 1 quando o item não está no inventário
                slotsWithItem++;
                itemList.Add(item);
            }
        }
        else
        {
            slotsWithItem++;
            itemList.Add(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }


    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType
                    && inventoryItem.itemName == item.itemName
                    && inventoryItem.itemSprite == item.itemSprite
                   )
                {
                    itemInInventory = inventoryItem;
                    itemInInventory.amount -= 1;
                    break;
                }
            }

            if (itemInInventory != null)
            {
                if (itemInInventory.amount <= 0)
                {
                    slotsWithItem--;
                    TooltipScreenSpaceUI.HideTooltip_Static();
                    itemList.Remove(itemInInventory);
                }
            }
        }
        else
        {
            slotsWithItem--;
            TooltipScreenSpaceUI.HideTooltip_Static();
            itemList.Remove(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }


    public void RemoveItemByAmount(Item item, int amount)
{
    // Verifica se o item está no inventário
    if (HasItem(item))
    {
        Item _itemInInventory = SearchItem(item);
        if (_itemInInventory != null && _itemInInventory.amount >= amount)
        {
            _itemInInventory.amount -= amount;

            // Se a quantidade chegar a zero ou menos, remova o item do inventário
            if (_itemInInventory.amount <= 0)
            {
                slotsWithItem--;
                itemList.Remove(_itemInInventory);
                TooltipScreenSpaceUI.HideTooltip_Static();
            }

            // Notifica a UI sobre a mudança no inventário
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.LogWarning("Quantidade insuficiente para remover");
        }
    }
    else
    {
        Debug.LogWarning("Inventário não contém este item");
    }
}


    public List<Item> GetItemList()
    {
        return itemList;
    }

    public bool CanPickup()
    {
        if (slotsWithItem >= maxSlots)
        {
            return false;
        }

        return true;
    }

    public bool HasItem(Item item)
    {
        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem == item) { return true; }
        }

        return false;
    }

    public Item SearchItem(Item item)
    {

        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem == item) { return inventoryItem; }
        }

        return null;
    }

    public int CountItem(Item item)
    {
        if (HasItem(item))
        {
            return SearchItem(item).amount;
        }

        return 0;
    }

}