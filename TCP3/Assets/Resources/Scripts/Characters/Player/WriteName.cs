using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class WriteName : NetworkBehaviour
{
    private TextMeshProUGUI textname;

    IEnumerator Start()
    {
        textname = GetComponentInChildren<TextMeshProUGUI>();

        if (IsServer)
        {
            while (NetworkManager.Singleton.ConnectedClients.Count != LobbyManager.Instance.joinedLobby.Players.Count)
            {
                yield return new WaitForSeconds(0.3f);
            }

            for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
            {
                NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponentInChildren<WriteName>().
                    SetPlayerNameClientRpc(LobbyManager.Instance.joinedLobby.Players[i].Data["PlayerName"].Value);
            }
        }

    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playername)
    { 
        textname.text = playername;
    }

    
}
