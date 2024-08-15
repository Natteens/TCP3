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
        Missao,
        Arma,
        Consumivel,
        Recurso,
        Dinheiro,
        Modulo,
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
            case Itemtype.Arma:
            case Itemtype.Missao:
            case Itemtype.Modulo:
            case Itemtype.None:
                return false;
            case Itemtype.Consumivel:
            case Itemtype.Recurso:
            case Itemtype.Dinheiro:
                return true;
        }
    }

}
