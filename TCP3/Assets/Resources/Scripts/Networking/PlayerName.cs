using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    public NetworkVariable<NetworkString> PlayerNickname = new NetworkVariable<NetworkString>();

    public TMP_Text playerNameText;
    public LobbyManager lobby;


    void Start()
    {
        playerNameText = GetComponentInChildren<TMP_Text>();
        lobby = FindObjectOfType<LobbyManager>();

       HandleNewPlayerJoined(OwnerClientId);
    }

    // Função que é chamada quando um novo jogador entra no jogo
    void HandleNewPlayerJoined(ulong clientId)
    {
        SetPlayerNameClientRpc(lobby.joinnedLobby.Players[(int)clientId].Data["name"].Value);
        Debug.Log("Me chamou?!");
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName)
    {
        PlayerNickname.Value = playerName;
        playerNameText.text = playerName;
        SetPlayerObjectName(playerName);
    }

    void SetPlayerObjectName(string playerName)
    {
        this.gameObject.name = playerName; // Altera o nome do GameObject pai       
    }
}
