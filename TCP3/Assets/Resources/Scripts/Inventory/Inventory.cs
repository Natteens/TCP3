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
                itemList.Remove(itemInInventory);
            }
        }
        else
        {
            currentSlots--;
            itemList.Remove(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
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

}
