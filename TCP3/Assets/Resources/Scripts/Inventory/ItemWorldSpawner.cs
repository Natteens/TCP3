using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ItemWorldSpawner : NetworkBehaviour
{
    public Item item;
    public float timeToSpawn = 5f;
    public float currentTime;

    private void Update()
    {
       CountToSpawn();
    }
    
    private void CountToSpawn()
    {
        if (currentTime > timeToSpawn)
        {
            SpawnRpc();
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SpawnRpc()
    {
        ItemWorld.DropItem(transform.position, item);
        currentTime = 0f;
    }
}
