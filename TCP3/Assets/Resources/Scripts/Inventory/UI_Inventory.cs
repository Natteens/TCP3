using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake()
    {
        itemSlotContainer = GameObject.Find("ItemSlotContainer").transform;
        itemSlotTemplate = itemSlotContainer.transform.Find("ItemSlotTemplate");
    }

    public void SetInventory(Inventory inventory)
    { 
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    //dxar privado dps
    public void RefreshInventoryItems()
    {
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
        img.sprite = item.itemSprite;
        txt.text = "x" + item.amount.ToString();
    }
}
