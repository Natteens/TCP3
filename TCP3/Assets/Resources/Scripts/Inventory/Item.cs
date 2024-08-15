using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Create Item")]
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

    [Header("Item Configs")]
    public string itemName;
    [Multiline] public string itemDescription;
    public Itemtype itemType;
    public Sprite itemSprite;
    //public GameObject itemModel;
    [HideInInspector] public int amount = 1;

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
