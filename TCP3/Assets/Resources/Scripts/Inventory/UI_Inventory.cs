using CodeMonkey.Utils;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Linq;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform inventoryHolder;
    [SerializeField] private RectTransform[] Slots;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private SlotExpandController expandController;
    private LocatePlayer player;
    public bool isVisible = false;

    private void Start()
    {
        inventoryHolder.gameObject.SetActive(false);
        RefreshInventoryItems();
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    public void UpdateItemPosition(int oldIndex, int newIndex)
    {
        List<Item> itemList = inventory.GetItemList();

        // Verifique se os índices estão dentro dos limites válidos
        if (oldIndex >= 0 && oldIndex < itemList.Count && newIndex >= 0 && newIndex < Slots.Length)
        {
            Item item = itemList[oldIndex];

            // Remove o item da posição antiga
            itemList[oldIndex] = null;

            // Se o novo índice está dentro dos limites da lista
            if (newIndex < itemList.Count)
            {
                if (itemList[newIndex] != null)
                {
                    SwapItemsInList(itemList, oldIndex, newIndex);   
                }

                itemList[newIndex] = item;
            }
            else
            {
                // Se o novo índice está fora dos limites da lista, adicione o item na última posição
                itemList.Add(item);
            }

            // Atualize a interface do usuário
            RefreshInventoryItems();
        }
    }
    public Inventory GetInventory() { return inventory; }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems()
    {
        List<Item> itemList = inventory.GetItemList();

        for (int i = 0; i < Slots.Length; i++)
        {
            if (i < itemList.Count && itemList[i] != null)
            {
                ConfigureItemSlot(itemList[i], Slots[i]);
            }
            else
            {
                if (Slots[i].childCount > 0)
                {
                    ClearSlot(Slots[i].GetChild(Slots[i].childCount - 1).GetComponent<RectTransform>());
                }
            }
        }
    }

    private void ConfigureItemSlot(Item item, RectTransform rect)
    {
        if (rect.childCount > 0)
        {
            ClearSlot(rect.GetChild(rect.childCount - 1).GetComponent<RectTransform>());
        }

        GameObject instance = Instantiate(itemSlotPrefab, rect);

        instance.GetComponent<Button_UI>().ClickFunc = () =>
        {
            Debug.Log("Meu item é:" + item.itemName);
            expandController.SetSlot(instance);
            expandController.Setup(item);

            if (item != null && item.itemType == Item.Itemtype.Consumivel)
            {
                expandController.hasUse = true;
            }

            expandController.ActiveItemButton();
            expandController.Expand();
            // Use Item
            //if (item != null)
            //{
            //    switch (item.itemType)
            //    {
            //        case Item.Itemtype.Consumivel:
            //            Consumable consumable = item as Consumable;
            //            if (consumable != null)
            //            {
            //                SurvivalManager manager = player.gameObject.GetComponent<SurvivalManager>();
            //                manager.IncreaseStats(consumable);
            //                inventory.RemoveItem(item);
            //            }
            //            break;
            //    }
            //}
        };

        instance.GetComponent<Button_UI>().MouseRightClickFunc = () =>
        {
            // Drop item
            //if (item != null)
            //{
            //    inventory.RemoveItem(item);
            //    ItemWorld.DropItem(player.GetPosition(), item);
            //}
        };

        // Configurar o item arrastável
        DraggableItem draggableItem = instance.GetComponent<DraggableItem>();
        draggableItem.SetItem(item);

        // Configurar a tooltip do item
        instance.GetComponent<ItemTooltip>().SetItem(item);

        Image img = instance.GetComponent<Image>();
        TextMeshProUGUI txt = instance.GetComponentInChildren<TextMeshProUGUI>();

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

    private void ClearSlot(RectTransform obj)
    {
        Destroy(obj.gameObject);
    }

    public LocatePlayer GetPlayer()
    {
        return player;
    }
    public void SetPlayer(LocatePlayer player)
    {
        this.player = player;
    }

    public void CheckVisibility()
    {
        isVisible = !isVisible;

        if (isVisible)
        {
            //Debug.Log("#Ativei o inventario#");
            GameManager.Instance.uiCraft.ControlExpandedCraft(false);
            expandController.Squeeze();
            expandController.Clean();
            MouseController.CursorVisibility(true);
        }
        else
        {
            //Debug.Log("#Desativei o inventario#");
            MouseController.CursorVisibility(false);
            TooltipScreenSpaceUI.HideTooltip_Static();
        }

        inventoryHolder.gameObject.SetActive(isVisible);
    }

    public void SwapItemsInList<T>(List<T> list, int indexA, int indexB)
    {
        if (indexA != indexB && indexA >= 0 && indexB >= 0 && indexA < list.Count && indexB < list.Count)
        {
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }
}
