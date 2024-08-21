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
    [SerializeField] private int currentSlots = 0;

    public Inventory()
    {
        itemList = new List<Item>();
    }

    public void AddItem(Item item)
    {
        if (CanPickup() == false) return;

        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += 1;
                    itemAlreadyInInventory = true;
                }
            }

            if (!itemAlreadyInInventory)
            {
                //Talvez de merda isso aq mas vamos ver!!
                item.amount = 1;
                currentSlots++;
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);
            currentSlots++;
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
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount -= 1;
                    itemInInventory = inventoryItem;
                }
            }

            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                currentSlots--;
                TooltipScreenSpaceUI.HideTooltip_Static();
                itemList.Remove(itemInInventory);
            }
        }
        else
        {
            currentSlots--;
            TooltipScreenSpaceUI.HideTooltip_Static();
            itemList.Remove(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItemByAmount(Item item, int amount)
    {
        if (HasItem(item))
        {
            foreach (Item _item in GetItemList())
            {
                if (_item == item)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        RemoveItem(item);
                    }
                }
            }

        }
        else
        {
            Debug.Log("Inventario sem esse item");
        }

    }


    public List<Item> GetItemList()
    {
        return itemList;
    }

    public bool CanPickup()
    {
        if (currentSlots >= maxSlots)
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

        return -1;
    }

}