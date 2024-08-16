using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerName : NetworkBehaviour
{
    // Variável de rede para armazenar o nome do jogador
    private NetworkVariable<string> playersName = new NetworkVariable<string>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsLocalPlayer)
        {
            // Defina o nome do jogador com base no LobbyManager
            string name = LobbyManager.Instance.GetName() != null ? LobbyManager.Instance.GetName() : $"Player {OwnerClientId}";

            // Atualize o valor da variável de rede
            playersName.Value = name;
        }

        // Defina o overlay com o nome sincronizado
        SetOverlay();
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = playersName.Value;
    }

    void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playersName.Value))
        {
            SetOverlay();
            overlaySet = true;
        }
    }
}
