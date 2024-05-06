using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu (menuName = "Item")]
public class BaseItem : ScriptableObject
{
    //Conferir com game designers dps
    public enum ItemTemplate
    {
        Ore,
        Food,
        Material,
        Weapon
    }


    [SerializeField] private string itemName;
    [SerializeField] private ItemTemplate type;
    //[SerializeField] private GameObject resourceModel;
    [SerializeField] private Sprite itemSprite;

    public string ItemName { get { return itemName; } }
    public Sprite ItemSprite { get { return itemSprite; } } 
    public ItemTemplate Type{ get { return type; } } 


}
