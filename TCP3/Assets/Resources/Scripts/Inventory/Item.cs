using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;

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

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemType);
        serializer.SerializeValue(ref itemName);
        serializer.SerializeValue(ref itemDescription);
        serializer.SerializeValue(ref amount);
    }
}
