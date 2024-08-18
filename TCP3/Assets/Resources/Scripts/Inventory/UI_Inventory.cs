using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform inventoryHolder;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    private LocatePlayer player;
    private bool isVisible = false;

    private void Awake()
    {
        itemSlotContainer = GameObject.Find("ItemSlotContainer").transform;
        itemSlotTemplate = itemSlotContainer.transform.Find("ItemSlotTemplate");
    }

    private void Start()
    {
        inventoryHolder.gameObject.SetActive(isVisible);
    }

    public void SetInventory(Inventory inventory)
    { 
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    public void CheckVisibility()
    {
        switch (isVisible)
        {
            case true:
                Debug.Log("#Desativei o inventario#");
                isVisible = false;
                MouseController.CursorVisibility(false);
                break;
            case false:
                Debug.Log("#Ativei o inventario#");
                isVisible = true;
                MouseController.CursorVisibility(true);
                break;
        }

        inventoryHolder.gameObject.SetActive(isVisible);
    }

    public void SetPlayer(LocatePlayer player)
    { 
        this.player = player;
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        //Adicionar popUp do item adicionada
        RefreshInventoryItems();
    }
    private void RefreshInventoryItems()
    {
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            ConfigureItemSlot(item, itemSlotRectTransform);
        }
    }

    private void ConfigureItemSlot(Item item, RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.gameObject.GetComponent<Button_UI>().ClickFunc = () => 
        {
            //Use item
        };

        rect.gameObject.GetComponent<Button_UI>().MouseRightClickFunc = () => 
        {
            //Drop item
            Debug.Log(item);
            inventory.RemoveItem(item);
            ItemWorld.DropItem(player.GetPosition(), item);
        };

        rect.gameObject.GetComponent<ItemTooltip>().SetItem(item);

        Image img = rect.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = rect.Find("amount").GetComponent<TextMeshProUGUI>();

        if(txt == null) { Debug.LogError("txt nao encontrado"); }
        if(img == null) { Debug.LogError("img nao encontrado"); }

        img.sprite = item.itemSprite;
        txt.text = item.amount.ToString();
        Debug.Log(item.itemName + ": x"+ item.amount.ToString());
    }
}
