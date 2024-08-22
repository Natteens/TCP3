using CodeMonkey.Utils;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform inventoryHolder;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    private List<RectTransform> itemSlots; // Lista de todos os slots criados
    private LocatePlayer player;
    private bool isVisible = false;

    private void Awake()
    {
        itemSlotContainer = GameObject.Find("ItemSlotContainer").transform;
        itemSlotTemplate = itemSlotContainer.transform.Find("ItemSlotTemplate");
        itemSlots = new List<RectTransform>();

        InitializeSlots();
    }

    private void Start()
    {
        inventoryHolder.gameObject.SetActive(isVisible);
    }

    private void InitializeSlots()
    {
        // Criar 30 slots vazios ao iniciar
        for (int i = 0; i < 30; i++)
        {
            RectTransform slotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            slotRectTransform.gameObject.SetActive(true);
            itemSlots.Add(slotRectTransform);
            ClearSlot(slotRectTransform); // Limpar visualmente o slot
        }
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
        // Limpar todos os slots antes de preencher novamente
        foreach (RectTransform slot in itemSlots)
        {
            ClearSlot(slot);
        }

        // Preencher slots com itens do inventário
        List<Item> itemList = inventory.GetItemList();
        for (int i = 0; i < itemList.Count; i++)
        {
            ConfigureItemSlot(itemList[i], itemSlots[i]);
        }
    }

    private void ConfigureItemSlot(Item item, RectTransform rect)
    {
        rect.gameObject.GetComponent<Button_UI>().ClickFunc = () =>
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

        rect.gameObject.GetComponent<Button_UI>().MouseRightClickFunc = () =>
        {
            // Drop item
            if (item != null)
            {
                inventory.RemoveItem(item);
                ItemWorld.DropItem(player.GetPosition(), item);
            }
        };

        // Configurar o item arrastável
        DraggableItem draggableItem = rect.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            draggableItem = rect.gameObject.AddComponent<DraggableItem>();
        }
        draggableItem.SetItem(item);

        // Configurar a tooltip do item
        rect.gameObject.GetComponent<ItemTooltip>().SetItem(item);

        Image img = rect.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = rect.Find("amount").GetComponent<TextMeshProUGUI>();

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


    private void ClearSlot(RectTransform rect)
    {
        // Limpar a imagem e texto do slot
        Image img = rect.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = rect.Find("amount").GetComponent<TextMeshProUGUI>();

        if (img != null) img.sprite = null;
        if (txt != null) txt.text = "";

        // Limpar o item do DraggableItem
        DraggableItem draggableItem = rect.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            draggableItem.SetItem(null);
        }
    }

}
