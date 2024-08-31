using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class EntitySpawner : NetworkBehaviour
{
    public string entityId;
    public float timeToSpawn = 50f;
    private float currentTime;
    [Range(1,250)]
    public float dropRadius;
    [Range(1, 100)]
    public byte minLevel;
    [Range(1, 100)]
    public byte maxLevel;

    //fazer lista de inimigos armazenando e ativar apenas se tiver um jogador na regiao do spawner para otimizar o jogo
    private void Update()
    {
        if (IsServer)
        {
            CountToSpawn();
        }
    }

    private void CountToSpawn()
    {
        if (GameManager.Instance.isNight.Value)
        {
            CountLogic(timeToSpawn / 2);
        }
        else
        {
            CountLogic(timeToSpawn);
        }
    }

    private void CountLogic(float _timeToSpawn)
    {
        if (currentTime > _timeToSpawn)
        {
            Vector3 randomOffset = new(Random.Range(-dropRadius, dropRadius), 0, Random.Range(-dropRadius, dropRadius));
            Vector3 spawnPosition = transform.position + randomOffset;

            Spawner.Instance.SpawnEntityInWorldServerRpc(spawnPosition, entityId, minLevel, maxLevel);
            currentTime = 0f;

        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }

}
