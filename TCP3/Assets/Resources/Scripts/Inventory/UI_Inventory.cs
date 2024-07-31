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
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake()
    {
        itemSlotContainer = GameObject.Find("ItemSlotContainer").transform;
        itemSlotTemplate = itemSlotContainer.transform.Find("ItemSlotTemplate");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryHolder.gameObject.SetActive(!inventoryHolder.gameObject.activeSelf);
        }
    }

    public void SetInventory(Inventory inventory)
    { 
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
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
        Image img = rect.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = rect.Find("amount").GetComponent<TextMeshProUGUI>();

        if(txt == null) { Debug.LogError("txt nao encontrado"); }
        if(img == null) { Debug.LogError("img nao encontrado"); }

        img.sprite = item.itemSprite;
        txt.text = "x" + item.amount.ToString();
        Debug.Log(item.itemName + ": x"+ item.amount.ToString());
    }
}
