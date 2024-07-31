using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
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
            ItemWorld.SpawnItemWorld(transform.position, item);
            currentTime = 0f;
        }
        else { currentTime += Time.deltaTime; }
    }
}
