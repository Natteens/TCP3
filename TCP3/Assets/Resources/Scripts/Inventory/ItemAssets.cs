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

    private Dictionary<string, GameObject> prefabDictionary;
    private void Start()
    {
        InitializeDictionary();
    }
    private void InitializeDictionary()
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

    public GameObject GetPrefabById(string id)
    {
        if (prefabDictionary.TryGetValue(id, out var prefab))
        {
            return prefab;
        }
        Debug.LogError($"Prefab with ID '{id}' not found.");
        return null;
    }
}
