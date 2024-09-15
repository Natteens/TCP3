using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Item/Create Consumable")]
public class Consumable : Item, INetworkSerializable
{
    [Header("Consumable Configs")]
    public ConsumableType type;
    public float restoreAmount;
    public List<StatusEffect> effects;

    private void OnValidate()
    {
        itemType = Itemtype.Consumivel;

        string newInfos = "<b>Restaura " + restoreAmount.ToString() + " de " + type.ToString() + "</b>";

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