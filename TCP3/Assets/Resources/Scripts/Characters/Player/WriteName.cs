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

    // Cria uma NetworkVariable para armazenar o nome do jogador
    private NetworkVariable<string> playerName = new NetworkVariable<string>();

    private void Awake()
    {
        textname = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Define a NetworkVariable com o nome do jogador
            playerName.Value = LobbyManager.Instance.GetName();
        }
    }

    private void Start()
    {
        // Atualiza o nome do jogador sempre que a NetworkVariable muda
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            textname.text = newValue;
        };
    }
}
