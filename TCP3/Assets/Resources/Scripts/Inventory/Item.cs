using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Item")]
public class Item: ScriptableObject
{
    public enum Itemtype
    { 
        Weapon,
        HealthPotion,
        Resource,
        Money
    }

    public string itemName;
    public Itemtype itemType;
    public Sprite itemSprite;
    public GameObject itemModel;
    [HideInInspector] public int amount;

}
