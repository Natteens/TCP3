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
            Spawner.Instance.SpawnItemServerRpc(transform.position, item.uniqueID);
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

}
