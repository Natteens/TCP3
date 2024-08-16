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
        if (IsServer) // Certifique-se de que o código é executado apenas no servidor
        {
            CountToSpawn();
        }
    }

    private void CountToSpawn()
    {
        if (currentTime > timeToSpawn)
        {
            SpawnItemServerRpc(transform.position, item.name); // Envia o nome ou ID do item
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    [ServerRpc]
    private void SpawnItemServerRpc(Vector3 position, string itemName)
    {
        // Procure o item pelo nome ou ID
       // Item item = ItemDatabase.Instance.GetItemByName(itemName); // Implemente o ItemDatabase para buscar o item
        ItemWorld.DropItem(position, item);
    }
}
