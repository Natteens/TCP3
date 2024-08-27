using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ItemAssets : Singleton<ItemAssets>
{

    [System.Serializable]
    public class PrefabEntry
    {
        [Tooltip("ID único para o prefab")]
        public string id;

        [Tooltip("Prefab a ser instanciado")]
        public GameObject prefab;
    }

    [Title("Prefabs")]
    [ListDrawerSettings(Expanded = true, DraggableItems = true)]
    public List<PrefabEntry> prefabEntries;

    [SerializeField] private Dictionary<string, GameObject> prefabDictionary;
    [SerializeField] private Dictionary<string, Item> itemDictionary;

    private void Awake()
    {
        InitializePrefabDictionary();
        InitializeItemDictionary();
    }

    private void InitializePrefabDictionary()
    {
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (var entry in prefabEntries)
        {
            if (!prefabDictionary.ContainsKey(entry.id))
            {
                prefabDictionary.Add(entry.id, entry.prefab);
            }
        }
    }

    private void InitializeItemDictionary()
    {
        itemDictionary = new Dictionary<string, Item>();

        // Carrega todos os ScriptableObjects do tipo Item
        Item[] items = Resources.LoadAll<Item>("DataBase");

        foreach (var item in items)
        {
            if (!itemDictionary.ContainsKey(item.uniqueID))
            {
                itemDictionary.Add(item.uniqueID, item);
            }
        }
    }

    public GameObject GetPrefabById(string id)
    {
        if (prefabDictionary.TryGetValue(id, out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"Prefab with ID '{id}' not found.");
        return null;
    }

    public Item GetItemById(string id)
    {
        if (itemDictionary.TryGetValue(id, out var item))
        {
            return item;
        }
        Debug.LogError($"Item with ID '{id}' not found.");
        return null;
    }
}
