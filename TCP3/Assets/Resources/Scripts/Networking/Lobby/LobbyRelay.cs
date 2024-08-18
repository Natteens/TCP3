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

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.Instance.GetJoinedLobby().MaxPlayers);

            if (allocation == null)
            {
                Debug.LogError("Failed to create relay allocation.");
                return null;
            }

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Failed to get join code.");
                return null;
            }

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


    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Attempting to join relay with join code: " + joinCode);
  
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log("Starting client...");
            NetworkManager.Singleton.StartClient();

            Debug.LogWarning("Client or server is already running.");
            
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: " + e.Message);
        }
    }



}
