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

    public void AddItem(Item newItem)
    {
        if (!CanPickup()) return;

        FeedbackManager.Instance.FeedbackItem(newItem);
        
        if (newItem.IsStackable())
        {
            // Tenta encontrar um item igual para acumular
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] != null && itemList[i].uniqueID == newItem.uniqueID)
                {
                    itemList[i].amount += newItem.amount;
                    OnItemListChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // Encontra o primeiro slot vazio e coloca o novo item l�
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null)
            {
                itemList[i] = newItem;
                slotsWithItem++;
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    public void RemoveItem(Item item)
    {
        TooltipScreenSpaceUI.HideTooltip_Static();

        int hotbarStartIndex = Math.Max(0, itemList.Count - 5);

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

                        // N�o reorganizar se o item estiver na hotbar
                        if (i < hotbarStartIndex)
                        {
                            ReorganizeInventory(i);
                        }
                    }
                }
                else
                {
                    itemList[i] = null;
                    slotsWithItem--;

                    // N�o reorganizar se o item estiver na hotbar
                    if (i < hotbarStartIndex)
                    {
                        ReorganizeInventory(i);
                    }
                }
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    private void ReorganizeInventory(int emptyIndex)
    {
        for (int i = emptyIndex; i < itemList.Count - 1; i++)
        {
            // N�o mover itens que est�o na hotbar
            if (i < itemList.Count - 5)
            {
                itemList[i] = itemList[i + 1];
                itemList[i + 1] = null;
            }
        }
    }

    public void RemoveItemByAmount(Item item, int amount)
    {
        TooltipScreenSpaceUI.HideTooltip_Static();
        // Verifica se o item est� no invent�rio
        if (HasItem(item))
        {
            Item _itemInInventory = SearchItem(item);
            if (_itemInInventory != null && _itemInInventory.amount >= amount)
            {
                
                for (int i = 0; i < amount; i++)
                {
                    _itemInInventory.amount -= 1;

                    if (_itemInInventory.amount <= 0)
                    {
                        slotsWithItem--;
                        itemList.Remove(_itemInInventory);
                        TooltipScreenSpaceUI.HideTooltip_Static();
                        break;
                    }
                }
                
                // Notifica a UI sobre a mudan�a no invent�rio
                OnItemListChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.LogWarning("Quantidade insuficiente para remover");
            }
        }
        else
        {
            Debug.LogWarning("Invent�rio n�o cont�m este item");
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

    public Item SearchItemByName(string name)
    {
        //espero que isso funcione
        return itemList.FirstOrDefault(i => i != null && i.itemName == name);
    }

    public int CountItem(Item item)
    {
        var foundItem = SearchItem(item);
        return foundItem != null ? foundItem.amount : 0;
    }
}
