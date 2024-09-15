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
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] != null && itemList[i].uniqueID == newItem.uniqueID)
                {
                    itemList[i].amount += 1;
                    OnItemListChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // Encontra o primeiro slot vazio e coloca o novo item lá
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

                        // Não reorganizar se o item estiver na hotbar
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

                    // Não reorganizar se o item estiver na hotbar
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

    public void RemoveItemByAmount(Item item, int amount)
    {
        TooltipScreenSpaceUI.HideTooltip_Static();

        for (int i = 0; i < amount; i++)
        {
            RemoveItem(item);
        }
    }

    public void DropRandomItem()
    {
        // Cria uma lista de índices de itens válidos (não nulos)
        List<int> validItemIndexes = new List<int>();

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null)
            {
                validItemIndexes.Add(i);
            }
        }

        // Se não houver itens, não faz nada
        if (validItemIndexes.Count == 0)
        {
            Debug.LogWarning("Nenhum item para remover");
            return;
        }

        // Escolhe um índice aleatório da lista de itens válidos
        int randomIndex = validItemIndexes[UnityEngine.Random.Range(0, validItemIndexes.Count)];

        // Crie uma cópia do item antes de alterar o valor
        Item itemToDrop = ScriptableObjectUtility.Clone(itemList[randomIndex]);

        Spawner.Instance.SpawnItemServerRpc(GameManager.Instance.uiInventory.GetPlayer().GetPosition(), itemToDrop.uniqueID);

        // Remove o item do índice aleatório
        RemoveItem(itemList[randomIndex]);
    }

    private void ReorganizeInventory(int emptyIndex)
    {
        for (int i = emptyIndex; i < itemList.Count - 1; i++)
        {
            // Não mover itens que estão na hotbar
            if (i < itemList.Count - 5)
            {
                itemList[i] = itemList[i + 1];
                itemList[i + 1] = null;
            }
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
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null && itemList[i].itemName == name)
            {
                return itemList[i];
            }
        }
        return null; // Retorna null se o item não for encontrado
    }

    public int CountItem(Item item)
    {
        var foundItem = SearchItem(item);
        return foundItem != null ? foundItem.amount : 0;
    }
}
