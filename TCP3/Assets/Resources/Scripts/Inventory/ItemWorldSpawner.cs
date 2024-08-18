using UnityEngine;
using Unity.Netcode;

public class ItemWorldSpawner : NetworkBehaviour
{
    public Item item;
    public float timeToSpawn = 5f;
    private float currentTime;

    private void Update()
    {
        if (IsServer)
        {
            CountToSpawn();
        }
    }

    private void CountToSpawn()
    {
        if (currentTime > timeToSpawn)
        {
            SpawnItemServerRpc(transform.position, item.name);
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemServerRpc(Vector3 position, string itemName)
    {
        ItemWorld.SpawnItemWorld(position, item);
    }
}
