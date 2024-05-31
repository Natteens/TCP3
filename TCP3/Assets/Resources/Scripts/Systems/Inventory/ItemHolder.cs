using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class ItemHolder : MonoBehaviour
{
    [Title("Details Section")]
    public BaseItem item;
    public int quantity;
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private Image itemSprite;
    [SerializeField] private GameObject slotExpand;

    [Space(20)]

    [Title("Expand Details Section")]
    [SerializeField] private TextMeshProUGUI detailsText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemQuantity_detail;
    private Managers manager;

    public void Start()
    {
        slotExpand.SetActive(false);
    }

    public void UpdateItem(BaseItem _item, int _quantity)
    {
        item = _item;

        detailsText.text = item.ItemDescription;
        itemNameText.text = item.ItemName;

        quantity = _quantity;
        itemSprite.sprite = item.ItemSprite;
    }

    public void Update()
    {
        if (enabled && item != null)
        {
            itemQuantity.text = "x" + quantity.ToString();
            itemQuantity_detail.text = "x" + quantity.ToString();
        }

       
    }

    public void Expand()
    {
        slotExpand.SetActive(true);
        slotExpand.GetComponent<Animator>().Play("SlotExpandAnim");
    }

    public void Retrait()
    {
        slotExpand.SetActive(false);
    }

    public void SetManager(Managers m)
    {
        manager = m;
    }

    public void DropItem() //colocar int i aqui dps quando for dropar por quant
    {
        manager.m_player.RemoveItem(item,1); //mudar aqui pra int i
    }

}
