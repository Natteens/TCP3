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
        if (IsServer) // Verifica se é o servidor
        {
            CountToSpawn();
        }
    }

    private void CountToSpawn()
    {
        if (currentTime > timeToSpawn)
        {
            // Chama o RPC no servidor para spawnar o item
            SpawnItemServerRpc(transform.position, item);
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    // RPC chamado no servidor para criar o item
    [ServerRpc]
    private void SpawnItemServerRpc(Vector3 position, Item item)
    {
        ItemWorld.DropItem(position, item);
    }
}
