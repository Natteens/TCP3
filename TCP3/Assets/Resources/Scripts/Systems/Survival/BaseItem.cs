using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NovoItem", menuName = "Itens/BaseItem")]
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
    [Multiline][SerializeField] private string itemDescription;
    [SerializeField] private ItemTemplate type;
    //[SerializeField] private GameObject resourceModel;
    [SerializeField] private Sprite itemSprite;

    public string ItemName { get { return itemName; } }
    public string ItemDescription { get { return itemDescription; } }
    public Sprite ItemSprite { get { return itemSprite; } } 
    public ItemTemplate Type{ get { return type; } } 

}
