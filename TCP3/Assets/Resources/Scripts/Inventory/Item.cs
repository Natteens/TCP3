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
        if (amount > 1 || amount <= 0)
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
        Debug.Log(amount);
        this.uniqueID = item.uniqueID;
        if (amount < 1) amount = 1;
        Debug.Log(amount);
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

        if (serializer.IsWriter)
        {
            byte[] textureBytes = SpriteToBytes(itemSprite);
            serializer.SerializeValue(ref textureBytes);
        }
        else
        {
            byte[] textureBytes = null;
            serializer.SerializeValue(ref textureBytes);
            itemSprite = BytesToSprite(textureBytes);
        }

        serializer.SerializeValue(ref itemDescription);
        serializer.SerializeValue(ref amount);
        Debug.Log("Serialized amount: " + amount);
        serializer.SerializeValue(ref uniqueID);
    }

    private byte[] SpriteToBytes(Sprite sprite)
    {
        if (sprite == null) return null;

        Texture2D texture = sprite.texture;
        return texture.EncodeToPNG(); // Ou EncodeToJPG se preferir
    }

    private Sprite BytesToSprite(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return null;

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
