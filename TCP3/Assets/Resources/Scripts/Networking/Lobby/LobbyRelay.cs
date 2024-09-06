using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LobbyRelay : MonoBehaviour
{
    public static LobbyRelay Instance { get; private set; }
    MonoBehaviour mono;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("Client or Server is already running.");
                return null;
            }

            var joinedLobby = LobbyManager.Instance.GetJoinedLobby();
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(joinedLobby.MaxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"RelayServiceException: {e.Message}");
            return null;
        }
    }


    public async void JoinRelay(string joinCode, MonoBehaviour _mono)
    {
        try
        {
            Debug.Log("joinrelay");
            mono = _mono;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.OnClientStarted -= Singleton_OnClientStarted;
            NetworkManager.Singleton.OnClientStarted += Singleton_OnClientStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Falha ao entrar no relay: " + e.Message);
        }
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        if (mono != null)
        {
            mono.enabled = false;
            Debug.Log("desativei");
        }
    }

    private void Singleton_OnClientStarted()
    {
        Debug.Log("Singleton_OnClientStarted");
    }
}
