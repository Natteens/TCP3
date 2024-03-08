using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField playerNameInput, lobbyCodeInput;
    public GameObject introLobby, lobbyPanel;
    public TMP_Text lobbyCodeText;
    public TMP_Text[] playerNameText;
    Lobby hostLobby, joinnedLobby;

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
        Debug.Log("Usu�rio logado como " + AuthenticationService.Instance.PlayerId);
    }

    async public void CreateLobby()
    {
        await Authenticate();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = GetPlayer()
        };

        hostLobby = await Lobbies.Instance.CreateLobbyAsync("lobby", 20, options);
        joinnedLobby = hostLobby;
        Debug.Log("Criou o lobby" + hostLobby.LobbyCode);
        InvokeRepeating("SendLobbyHeartBeat", 10, 10);
        UpdateLobby();
        ShowPlayer();

        lobbyCodeText.text = joinnedLobby.LobbyCode;
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
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
        ShowPlayer();

        lobbyCodeText.text = joinnedLobby.LobbyCode;
        introLobby.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    async void SendLobbyHeartBeat()
    {
        if(hostLobby == null)
            return;
        await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        Debug.Log("Atualizou o Lobby");
        ShowPlayer();
    }

    void ShowPlayer()
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
}
