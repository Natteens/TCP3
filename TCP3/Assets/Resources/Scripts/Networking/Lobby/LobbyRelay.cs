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
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            // Espera até que o host seja iniciado corretamente antes de prosseguir
            float timeout = 10f;
            while (!NetworkManager.Singleton.IsListening && timeout > 0)
            {
                await Task.Delay(100);
                timeout -= 0.1f;
            }

            if (!NetworkManager.Singleton.IsListening)
            {
                Debug.LogError("Failed to start host.");
                return null;
            }

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

            // Espera até que o cliente seja conectado corretamente antes de prosseguir
            float timeout = 10f;
            while (!NetworkManager.Singleton.IsConnectedClient && timeout > 0)
            {
                await Task.Delay(100);
                timeout -= 0.1f;
            }

            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.LogError("Failed to connect as client.");
                // Aqui você pode adicionar alguma lógica para lidar com a falha de conexão
            }

        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: " + e.Message);
        }
    }



}
