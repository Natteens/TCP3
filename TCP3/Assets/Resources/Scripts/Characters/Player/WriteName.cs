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
    [SerializeField] private TextMeshProUGUI textname;

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            SendPlayerNameToServerRpc(LobbyManager.Instance.joinedLobby.Players[(int)NetworkManager.Singleton.LocalClientId].Data["PlayerName"].Value);
        }
    }

    [ServerRpc]
    public void SendPlayerNameToServerRpc(string playerName, ServerRpcParams rpcParams = default)
    {
        SetPlayerNameClientRpc(playerName);
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName, ClientRpcParams rpcParams = default)
    {
        textname.text = playerName;
    }
}
