using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    public BaseItem item;
    public int quantity;
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private Image itemSprite;

    public void UpdateItem(BaseItem _item, int _quantity)
    {
        item = _item;
        quantity = _quantity;
        itemSprite.sprite = item.ItemSprite;
    }

    public void Update()
    {
        if (enabled && item != null)
        {
            itemQuantity.text = quantity.ToString();
        }
    }
}
