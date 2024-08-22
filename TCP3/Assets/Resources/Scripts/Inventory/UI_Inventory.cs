using CodeMonkey.Utils;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Linq;
using Unity.VisualScripting;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform inventoryHolder;
    [SerializeField] private Transform inventorySlotContainer;
    [SerializeField] private Transform[] Slots;
    [SerializeField] private GameObject itemSlotPrefab; 
    private LocatePlayer player;
    private bool isVisible = false;

    private void Start()
    {
        inventoryHolder.gameObject.SetActive(isVisible);
        RefreshInventoryItems();
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    public LocatePlayer GetPlayer()
    {
        return player;
    }

    public void CheckVisibility()
    {
        isVisible = !isVisible;

        if (isVisible)
        {
            //Debug.Log("#Ativei o inventario#");
            GameManager.Instance.uiCraft.ControlExpandedCraft(false);
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

    public void SetPlayer(LocatePlayer player)
    {
        this.player = player;
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems()
    {
        // Obter a lista de itens do inventário
        List<Item> itemList = inventory.GetItemList();
       
        // Preencher os slots com itens disponíveis
        for (int i = 0; i < itemList.Count && i < Slots.Length; i++)
        {
            ConfigureItemSlot(itemList[i], Slots[i]);
        }
    }

    private void ConfigureItemSlot(Item item, Transform rect)
    {
        if (rect.childCount > 0)
        {
            ClearSlot(rect.GetChild(rect.childCount - 1).gameObject);
        }

        GameObject instance = Instantiate(itemSlotPrefab, rect);

        instance.GetComponent<Button_UI>().ClickFunc = () =>
        {
            // Use Item
            if (item != null)
            {
                switch (item.itemType)
                {
                    case Item.Itemtype.Consumivel:
                        Consumable consumable = item as Consumable;
                        if (consumable != null)
                        {
                            SurvivalManager manager = player.gameObject.GetComponent<SurvivalManager>();
                            manager.IncreaseStats(consumable);
                            inventory.RemoveItem(item);
                        }
                        else
                        {
                            Debug.LogWarning("CASTING NAO ESTA FUNCIONANDO");
                        }
                        break;
                }
            }
        };

        instance.GetComponent<Button_UI>().MouseRightClickFunc = () =>
        {
            // Drop item
            if (item != null)
            {
                inventory.RemoveItem(item);
                ItemWorld.DropItem(player.GetPosition(), item);
            }
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

    private void ClearSlot(GameObject obj)
    {
        Debug.Log("destruindo slot:" + obj.name);
        Destroy(obj);
    }

}
