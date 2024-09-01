using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class EntitySpawner : NetworkBehaviour
{
    public string entityId;
    public float timeToSpawn = 50f;
    public int maxSpawns = 25;
    public int currentSpawns = 0;
    private float currentTime;
    public LayerMask layerPlayer;
    public LayerMask layerEnemy;
    [Range(1,250)]
    public float spawnRadius;
    [Range(1, 250)]
    public int checkRadius;
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
            Vector3 randomOffset = new(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
            Vector3 spawnPosition = transform.position + randomOffset;


            if (currentSpawns >= maxSpawns)
            {
                if(!HasNearPlayers()) ClearAllEnemys();
                return;
            }

            if (HasNearPlayers())
            {
                Spawner.Instance.SpawnEntityInWorldServerRpc(spawnPosition, entityId, minLevel, maxLevel);
                currentSpawns++;
            }

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
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    private bool HasNearPlayers()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius, layerPlayer);

        foreach (Collider hit in hits)
        {
            Debug.Log("Objeto detectado: " + hit.name);
            Debug.DrawLine(transform.position, hit.transform.position, Color.red);
            return true;
        }

        return false;
    }

    private void ClearAllEnemys()
    {
        currentSpawns = 0;
        currentTime = 0f;

        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius, layerEnemy);

        foreach (Collider hit in hits)
        {
            NetworkObject networkObj = hit.gameObject.GetComponent<NetworkObject>();

            if (networkObj != null) 
            {
                networkObj.Despawn();
            }

            Destroy(hit.gameObject);
        }
    }

}
