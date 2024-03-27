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
    private string playerName;

    private void Awake()
    {
        textname = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Envia o nome do jogador para o servidor
            SetPlayerNameServerRpc(LobbyManager.Instance.GetName());
        }
    }

    [ServerRpc]
    private void SetPlayerNameServerRpc(string playername, ServerRpcParams rpcParams = default)
    {
        // Armazena o nome do jogador
        playerName = playername;

        // Envia o nome do jogador para todos os clientes
        SetPlayerNameClientRpc(playername);
    }

    [ClientRpc]
    private void SetPlayerNameClientRpc(string playername, ClientRpcParams rpcParams = default)
    {
        // Atualiza o nome do jogador no objeto TextMeshProUGUI
        textname.text = playername;
    }

    private void Start()
    {
        // Define o nome do jogador quando o jogo começa
        textname.text = playerName;
    }
}
