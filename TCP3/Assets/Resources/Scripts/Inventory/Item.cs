using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Item")]
[System.Serializable]
public class Item: ScriptableObject
{
    public enum Itemtype
    {
        Quest,
        Weapon,
        Consumable,
        Resource,
        Money,
        Module,
        None
    }

    public string itemName;
    [Multiline] public string itemDescription;
    public Itemtype itemType;
    public Sprite itemSprite;
    //public GameObject itemModel;
    public int amount;

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case Itemtype.Weapon:
            case Itemtype.Quest:
            case Itemtype.Module:
                return false;
            case Itemtype.Consumable:
            case Itemtype.Resource:
            case Itemtype.Money:
                return true;
        }
    }

}
