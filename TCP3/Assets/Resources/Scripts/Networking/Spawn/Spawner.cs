using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    #region Item

    [ServerRpc(RequireOwnership = false)]
    public void SpawnItemServerRpc(Vector3 position, string itemId)
    {
        SpawnItemWorld(position, itemId); 
    }

    private void SpawnItemWorld(Vector3 position, string itemId)
    {
        int dropRadius = 3;
        Vector3 randomOffset = new(Random.Range(-dropRadius, dropRadius), 0, Random.Range(-dropRadius, dropRadius));
        Vector3 finalDropPosition = position + randomOffset;

        var itemWorldInstance = Instantiate(ItemAssets.Instance.GetPrefabById("itemWorld"), finalDropPosition, Quaternion.identity);

        itemWorldInstance.GetComponent<ItemWorld>().SetItem(ItemAssets.Instance.GetItemById(itemId));

        if (itemWorldInstance.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.Spawn();
        }
    }

    #endregion

    #region Objects

    [ServerRpc(RequireOwnership = false)]
    public void SpawnInWorldServerRpc(Vector3 position, string prefabId)
    {
        SpawnInWorld(position, prefabId);
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

    #endregion

    #region Entitys

    [ServerRpc(RequireOwnership = false)]
    public void SpawnEntityInWorldServerRpc(Vector3 position, string prefabId, byte minLevel, byte maxLevel)
    {
        SpawnEntityInWorld(position, prefabId, minLevel, maxLevel);
    }

    private void SpawnEntityInWorld(Vector3 position, string prefabId, byte minLevel, byte maxLevel)
    {
        var prefab = ItemAssets.Instance.GetPrefabById(prefabId);
        if (prefab == null)
        {
            return;
        }

        var worldInstance = Instantiate(prefab, position, Quaternion.identity);
        worldInstance.GetComponent<EnemySettings>().Setup(minLevel, maxLevel);

        if (worldInstance.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.Spawn();
        }
    }

    #endregion




}
