using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    private HashSet<NetworkObject> spawnedObjects = new HashSet<NetworkObject>();

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
            // Adiciona o objeto ao HashSet
            spawnedObjects.Add(networkObject);
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
            // Adiciona o objeto ao HashSet
            spawnedObjects.Add(networkObject);
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
            // Adiciona o objeto ao HashSet
            spawnedObjects.Add(networkObject);
        }
    }

    #endregion

    #region Projectiles

    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectilesServerRpc(Vector3 position, Vector3 shootDirection, string prefabId, int damage)
    {
        SpawnProjectiles(position, shootDirection, prefabId, damage);
    }

    private void SpawnProjectiles(Vector3 position, Vector3 shootDirection, string prefabId, int damage)
    {
        var prefab = ItemAssets.Instance.GetPrefabById(prefabId);
        if (prefab == null)
        {
            return;
        }

        var projectile = Instantiate(prefab, position, Quaternion.LookRotation(shootDirection, Vector3.up));
        projectile.GetComponent<ProjectileMover>().InitializeProjectile(damage);

        if (projectile.TryGetComponent<NetworkObject>(out var networkObject))
        {
            networkObject.Spawn();
            // Adiciona o objeto ao HashSet
            spawnedObjects.Add(networkObject);
        }
    }

    #endregion

    #region Despawn

    public void DespawnInWorld(NetworkObject obj)
    {
        if (spawnedObjects.Contains(obj))
        {
            obj.Despawn();
            spawnedObjects.Remove(obj); // Remove o objeto do HashSet
        }
    }

    public void DespawnByTimeInWorld(NetworkObject obj, float time)
    {
        StartCoroutine(DespawnByTime(obj, time));
    }

    private IEnumerator DespawnByTime(NetworkObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (spawnedObjects.Contains(obj))
        {
            obj.Despawn();
            spawnedObjects.Remove(obj); // Remove o objeto do HashSet
        }
    }

    // Método para despawnar todos os objetos
    public void DespawnAll()
    {
        foreach (var networkObject in spawnedObjects)
        {
            networkObject.Despawn();
        }
        spawnedObjects.Clear(); // Limpa o HashSet após despawnar todos os objetos
    }

    #endregion
}
