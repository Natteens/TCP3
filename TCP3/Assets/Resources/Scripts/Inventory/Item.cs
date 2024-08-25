using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Create Item")]
[System.Serializable]
public class Item : ScriptableObject, INetworkSerializable
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
    [PreviewField(100, ObjectFieldAlignment.Center)] public Sprite itemSprite;
    [ReadOnly] public int amount = 1;
    [ReadOnly] public string uniqueID; // Novo campo uniqueID

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            itemName = "Default Item Name"; // ou algum valor padrão
        }

        if (string.IsNullOrEmpty(uniqueID))
            uniqueID = GenerateUniqueID();  
        if (amount > 1)
        {
            amount = 1;
        }
    }

    public void Initialize(Item item)
    { 
        this.itemName = item.itemName;
        this.itemDescription = item.itemDescription;
        this.itemType = item.itemType;
        this.itemSprite = item.itemSprite;
        this.amount = item.amount;
        this.uniqueID = item.uniqueID;
    }

    private string GenerateUniqueID()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogError("Item name is not set before generating unique ID!");
            return null;
        }

        return Guid.NewGuid().ToString("N") + "_" + itemName.GetHashCode() + "_" + itemType.GetHashCode();
    }

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

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemType);
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref itemDescription);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref uniqueID); // Serialize uniqueID
    }
}
