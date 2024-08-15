using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private Item item;

    public void SetItem(Item _item)
    {
        item = _item;
    }

    public void Start()
    {
        if (item != null) {

            Button_UI ui = transform.GetComponent<Button_UI>();

            ui.hoverBehaviour_Sprite_Enter = item.itemSprite;
            ui.hoverBehaviour_Sprite_Exit = item.itemSprite;

            ui.MouseOverOnceFunc = () =>
            {
                string myTooltip = "<b><color=#00ff00>" + item.itemName + "</b></color>\n<color=#F00>"
                + "</color><i>" + item.itemType.ToString() + "</i>\n" + item.itemDescription;

                TooltipScreenSpaceUI.ShowTooltip_Static(myTooltip);
            };

            ui.MouseOutOnceFunc = () =>
            {
                TooltipScreenSpaceUI.HideTooltip_Static();
            };
        }
    }
}
