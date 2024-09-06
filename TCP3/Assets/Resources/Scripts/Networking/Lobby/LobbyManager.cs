using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    //Meu Lobby Manager

    public static LobbyManager Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_START_GAME = "PlayerName";

    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    // public event EventHandler OnGameStarted;

    private Lobby hostLobby;
    public Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private float refreshLobbyListTimer = 5f;

    private string playerName;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Logou: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //playerName = "carlos" + UnityEngine.Random.Range(10, 99);
        //Debug.Log(playerName);
    }

    private void Update()
    {
        if (joinedLobby == null)
        {
            HandleRefreshLobbyList();
            HandleLobbyHeartbeat(); 
        }
        else
        {
            HandleLobbyPollForUpdates(); 
        }
    }

    public string GetKeyGame()
    {
        return KEY_START_GAME;
    }

    private async void HandleLobbyPollForUpdates()
    {
        try
        {
            if (joinedLobby != null)
            {
                lobbyUpdateTimer -= Time.deltaTime;
                if (lobbyUpdateTimer < 0f)
                {
                    Debug.Log("agora vai!!");
                    float lobbyPollTimerMax = 1.2f;
                    lobbyUpdateTimer = lobbyPollTimerMax;

                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    // Mitigate host lobby
                    if (hostLobby != null && joinedLobby.HostId == GetAuthenticatedPlayerId())
                    {
                        hostLobby = joinedLobby;
                    }

                    if (!IsPlayerInLobby())
                    {
                        Debug.Log("Kicked from Lobby!");

                        OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                        joinedLobby = null;
                    }

                    if (joinedLobby.Data[KEY_START_GAME].Value != "0")
                    {
                        if (!IsLobbyHost())
                        {
                            enabled = false;
                            Debug.Log("lobbypoll");
                            LobbyRelay.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                        }
                    }
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning("Failed to poll for lobby refresh");
            Debug.Log(e);
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        try
        {
            LobbyUI.Instance.ControlLoadForClients(false);
            Player player = GetPlayer();

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            hostLobby = lobby;
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
           

            if (IsLobbyHost())
            {
                Debug.Log("Ativei o botao para o host!");
                LobbyUI.Instance.ControlStartButton(true);
            }

            Debug.Log("Created Lobby " + lobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void StartGame()
    {
       try
       {
            if (!IsLobbyHost())
            {
                LobbyUI.Instance.ControlLoadForClients(true);
                return;
            } 
          
            await LoadingGameScreen();
           
            string relayCode = await LobbyRelay.Instance.CreateRelay();
           
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode)} //muda a visibilidade aq
                }
           
            });
           
            joinedLobby = lobby;
           
       }
       catch (LobbyServiceException e)
       {
           Debug.Log(e);
       }
   
    }

    private async Task LoadingGameScreen()
    {
       await Loader.Load(gameScenes.Katalisya); // VOLTAR PRA CENA DE KATALISIA
    }

    public string GetName()
    {
        return playerName;
    }

    public string GetAuthenticatedPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    private void HandleRefreshLobbyList()
    {

        try
        {
            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
            {
                refreshLobbyListTimer -= Time.deltaTime;
                if (refreshLobbyListTimer < 0f)
                {
                    float refreshLobbyListTimerMax = 5f;
                    refreshLobbyListTimer = refreshLobbyListTimerMax;

                    RefreshLobbyList();
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filtros para mostrar apenas lobby aberto
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Ordem mais novo para o mais velho 
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            LobbyUI.Instance.ControlLoadForClients(false);
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;
            Debug.Log("Entrou no lobby com o codigo: " + lobbyCode);

            if (!IsLobbyHost())
            {
                LobbyUI.Instance.ControlStartButton(false);
            }

            PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby(Lobby lobby)
    {
        try
        {
            LobbyUI.Instance.ControlLoadForClients(false);
            Player player = GetPlayer();

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
            {
                Player = player
            });

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
       


            if (!IsLobbyHost())
            {
                LobbyUI.Instance.ControlStartButton(false);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();

            if (!IsLobbyHost())
            {
                LobbyUI.Instance.ControlStartButton(false);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
        });
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Jogadores no Lobby [" + lobby.Name + "]");
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    public async void UpdateLobbyMaxPlayers(int _maxPlayers)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                MaxPlayers = _maxPlayers
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdateLobbyName(string _nome)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Name = _nome
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerName(string playerName)
    {
        try
        {
            this.playerName = playerName;


            if (joinedLobby != null)
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;
                
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
         }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }  
    }

    public async void LeaveLobby()
    {
        try
        {
            if (joinedLobby != null)
            {
                if (joinedLobby.Players.Count > 1)
                {
                    if (IsLobbyHost()) { hostLobby = null; MigrateLobbyHost(); }
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    joinedLobby = null;
                    OnLeftLobby?.Invoke(this, EventArgs.Empty);
                    return;
                }
                else
                {
                    DeleteLobby();
                    return;
                } 
            }

            Debug.LogWarning("JOINED LOBBY NULO");
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void MigrateLobbyHost()
    {
        try
        {
            if (joinedLobby.Players[1].Id != null)
            {
                hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    HostId = joinedLobby.Players[1].Id
                });

                Debug.Log("um novo host foi setado!");
                joinedLobby = hostLobby;

                if (IsLobbyHost())
                {
                    LobbyUI.Instance.ControlStartButton(true);
                }

            }
            else
            {
                Debug.LogWarning("NEXT HOST ESTA NULO!");

            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
           if (hostLobby != null)
           {
               Debug.Log($"Tentando deletar o lobby com ID: {hostLobby.Id}");
               if (string.IsNullOrEmpty(hostLobby.Id))
               {
                   Debug.LogWarning("ID do lobby é nulo ou vazio.");
                   return;
               }

               var existingLobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
               if (existingLobby == null)
               {
                   Debug.LogWarning("Lobby não encontrado, pode já ter sido excluído.");
                   return;
               }

               await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
               hostLobby = null;
               joinedLobby = null;
               OnLeftLobby?.Invoke(this, EventArgs.Empty);
               Debug.Log("Lobby deletado com sucesso!");
           }
           else
           {
               Debug.LogWarning("HOST LOBBY NULO");
           }
         }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

    
    }

    private void OnApplicationQuit()
    {
        DeleteLobby();
    }
}