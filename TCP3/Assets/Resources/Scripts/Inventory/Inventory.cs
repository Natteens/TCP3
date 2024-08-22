using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Inventory
{
    public event EventHandler OnItemListChanged;
    [SerializeField] private List<Item> itemList;
    [SerializeField] private int maxSlots = 30;
    [SerializeField] private int slotsWithItem = 0;

    public Inventory()
    {
        itemList = new List<Item>(new Item[maxSlots]);
    }

    public void AddItem(Item item)
    {
        if (!CanPickup()) return;

        if (item.IsStackable())
        {
            // Tenta encontrar um item igual para acumular
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] != null && itemList[i].uniqueID == item.uniqueID)
                {
                    itemList[i].amount += 1;
                    OnItemListChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // Encontra o primeiro slot vazio e coloca o item lá
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null)
            {
                itemList[i] = item;
                slotsWithItem++;
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    public void RemoveItem(Item item)
    {
        TooltipScreenSpaceUI.HideTooltip_Static();

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null && itemList[i].uniqueID == item.uniqueID)
            {
                if (itemList[i].IsStackable())
                {
                    itemList[i].amount -= 1;
                    if (itemList[i].amount <= 0)
                    {
                        itemList[i] = null;
                        slotsWithItem--;
                    }
                }
                else
                {
                    itemList[i] = null;
                    slotsWithItem--;
                }
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    public void RemoveItemByAmount(Item item, int amount)
    {
        TooltipScreenSpaceUI.HideTooltip_Static();
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
        return slotsWithItem < maxSlots;
    }

    public bool HasItem(Item item)
    {
        return itemList.Any(i => i != null && i.uniqueID == item.uniqueID);
    }

    public Item SearchItem(Item item)
    {
        return itemList.FirstOrDefault(i => i != null && i.uniqueID == item.uniqueID);
    }

    public int CountItem(Item item)
    {
        var foundItem = SearchItem(item);
        return foundItem != null ? foundItem.amount : 0;
    }
}
