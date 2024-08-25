using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Item/Create Consumable")]
public class Consumable : Item
{
    [Header("Consumable Configs")]
    public ConsumableType type;
    public float restoreAmount;
    public List<StatusEffect> effects;

    private void OnValidate()
    {
        itemType = Itemtype.Consumivel;

        string newInfos = "<color=red>Restaura " + restoreAmount.ToString() + " de " + type.ToString() + "</color>";

        itemDescription = newInfos;
    }
}

public enum ConsumableType
{
    Fome,
    Sede,
    Vida,
    Energia
}