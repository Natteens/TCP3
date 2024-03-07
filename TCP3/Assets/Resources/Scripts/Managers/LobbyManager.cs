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
    Lobby hostLobby, joinnedLobby;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    async Task Authenticate()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Usuário logado como " + AuthenticationService.Instance.PlayerId);
    }

    async public void CreateLobby()
    {
        await Authenticate();
        hostLobby = await Lobbies.Instance.CreateLobbyAsync("lobby", 20);
        Debug.Log("Criou o lobby" + hostLobby.LobbyCode);
    }

    async public void JoinLobbyByCode()
    {
        await Authenticate();
        joinnedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text);
        Debug.Log("Entrou no lobby" + joinnedLobby.LobbyCode);
    }
}
