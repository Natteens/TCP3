using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkVFXInstance : MonoBehaviour
{
    public float timer = 1f;
    NetworkObject netObj;
    
    private void Start()
    {
        netObj = GetComponent<NetworkObject>();
        Despawn();
    }

    private void Despawn()
    {
        Spawner.Instance.DespawnByTimeInWorld(netObj, timer);
    }
}
