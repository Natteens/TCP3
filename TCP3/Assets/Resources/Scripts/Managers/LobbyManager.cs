using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UNET;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField playerNameInput, lobbyCodeInput;
    public GameObject introLobby, lobbyPanel;
    public TMP_Text lobbyCodeText;
    public TMP_Text[] playerNameText;
    public Lobby hostLobby, joinnedLobby;

    public GameObject startGameButton;
    public bool startedGame;

    private static LobbyManager _instance;
    public static LobbyManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    async Task Authenticate()
    {
        if(AuthenticationService.Instance.IsSignedIn)
         return; 

        AuthenticationService.Instance.ClearSessionToken();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Usuário logado como " + AuthenticationService.Instance.PlayerId);
    }

    async public void CreateLobby()
    {
        await Authenticate();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = GetPlayer(),
            Data = new Dictionary<string, DataObject>
            {
                {"StartGame", new DataObject(DataObject.VisibilityOptions.Member, "0") }
            }
        };

        hostLobby = await Lobbies.Instance.CreateLobbyAsync("lobby", 20, options);
        joinnedLobby = hostLobby;
        Debug.Log("Criou o lobby" + hostLobby.LobbyCode);
        InvokeRepeating("SendLobbyHeartBeat", 5,5);
        UpdateLobby();
        ShowPlayers();

        lobbyCodeText.text = joinnedLobby.LobbyCode;
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
        startGameButton.SetActive(true);
    }

    void CheckForUpdates()
    {
        if(joinnedLobby == null || startedGame) return;

        UpdateLobby();
        ShowPlayers();

        if (joinnedLobby.Data["StartGame"].Value != "0")
        {
            if (hostLobby == null)
            {
                JoinRelay(joinnedLobby.Data["StartGame"].Value);
            }
            startedGame = true;
        }
    }

    Player GetPlayer()
    {
        Player player = new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerNameInput.text) }
            }
        };
        return player;
    }

    async public void JoinLobbyByCode()
    {
        await Authenticate();

        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
        {
            Player = GetPlayer()
        };

        joinnedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text, options);
        Debug.Log("Entrou no lobby" + joinnedLobby.LobbyCode);
        UpdateLobby();
        ShowPlayers();

        lobbyCodeText.text = joinnedLobby.LobbyCode;
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
        InvokeRepeating("CheckForUpdates", 5, 5);
    }

    async void SendLobbyHeartBeat()
    {
        if(hostLobby == null || startedGame)
            return;
        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        Debug.Log("Atualizou o Lobby");
        UpdateLobby();
        ShowPlayers();

    }

    void ShowPlayers()
    {
        for(int i = 0; i<joinnedLobby.Players.Count; i++)
        {
            playerNameText[i].text = joinnedLobby.Players[i].Data["name"].Value;
        }
    }

    async void UpdateLobby()
    {
        if(joinnedLobby == null) return;

        joinnedLobby = await LobbyService.Instance.GetLobbyAsync(joinnedLobby.Id);
    }

    async Task<string> CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(20);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        return joinCode;
    }

    public async void StartGame()
    {
        GetComponent<sceneManager>().LoadGame();
        string relayCode = await CreateRelay();
        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinnedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
        {
            {"StartGame", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
        }
        });
        joinnedLobby = lobby;
        startedGame = true;
    }


    public void RefreshRoom()
    {
        if (hostLobby == null || startedGame)
            return;
        Debug.Log("Atualizou o Lobby");
        UpdateLobby();
        ShowPlayers();
    }

    async void JoinRelay(string joinCode)
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();

       // lobbyPanel.SetActive(false);
    }
}
