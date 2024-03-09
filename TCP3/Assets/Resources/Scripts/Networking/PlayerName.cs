using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    public TMP_Text playerNameText;
    public LobbyManager lobby;

    IEnumerator Start()
    {
        playerNameText = GetComponentInChildren<TMP_Text>();
        lobby = FindObjectOfType<LobbyManager>();

        if (IsServer)
        {
            while (NetworkManager.Singleton.ConnectedClients.Count != lobby.joinnedLobby.Players.Count)
                yield return new WaitForSeconds(0.5f);
            
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
            {
                NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponentInChildren<PlayerName>().
                    SetPlayerNameClientRpc(lobby.joinnedLobby.Players[i].Data["name"].Value);
            }
        }
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName)
    {
        playerNameText.text = playerName;
    }

}
