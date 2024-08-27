using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{

    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemServerRpc(Vector3 position, Item item)
    {
        Debug.Log(item);
        SpawnItemWorld(position, item); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnInWorldServerRpc(Vector3 position, string prefabId)
    {
        SpawnInWorld(position, prefabId);
    }

    private void SpawnItemWorld(Vector3 position, Item item)
    {
        Debug.Log(item);
        int dropRadius = 3;
        Vector3 randomOffset = new(Random.Range(-dropRadius, dropRadius), 0, Random.Range(-dropRadius, dropRadius));
        Vector3 finalDropPosition = position + randomOffset;

        var itemWorldInstance = Instantiate(ItemAssets.Instance.GetPrefabById("itemWorld"), finalDropPosition, Quaternion.identity);
        
        itemWorldInstance.GetComponent<ItemWorld>().SetItem(item);

        if (itemWorldInstance.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.Spawn();
        }
    }

    private void SpawnInWorld(Vector3 position, string prefabId)
    {
        var prefab = ItemAssets.Instance.GetPrefabById(prefabId);
        if (prefab == null)
        {
            return;
        }

        var worldInstance = Instantiate(prefab, position, Quaternion.identity);

        if (worldInstance.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.Spawn();
        }
    }
}
