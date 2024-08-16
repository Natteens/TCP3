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
            ItemWorld.DropItem(transform.position, item);
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
