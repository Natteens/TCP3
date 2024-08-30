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
            var joinedLobby = LobbyManager.Instance.GetJoinedLobby();
            if (joinedLobby == null)
            {
                Debug.LogError("Não há lobby para criar um Relay.");
                return null;
            }

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(joinedLobby.MaxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            // Verifica o timeout para iniciar o host corretamente
            float timeout = 10f;
            while (!NetworkManager.Singleton.IsListening && timeout > 0)
            {
                await Task.Delay(100);
                timeout -= 0.1f;
            }

            if (!NetworkManager.Singleton.IsListening)
            {
                Debug.LogError("Falha ao iniciar o host.");
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
            Debug.Log("Tentando entrar no relay com o código: " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log("Iniciando cliente...");
            NetworkManager.Singleton.StartClient();

            // Verifica o timeout para conectar como cliente
            float timeout = 10f;
            while (!NetworkManager.Singleton.IsConnectedClient && timeout > 0)
            {
                await Task.Delay(100);
                timeout -= 0.1f;
            }

            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.LogError("Falha ao conectar como cliente.");
                // Aqui você pode adicionar alguma lógica para lidar com a falha de conexão
            }

        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Falha ao entrar no relay: " + e.Message);
        }
    }
}
