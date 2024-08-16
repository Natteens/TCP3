using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Necessário para FixedString

public class PlayerName : NetworkBehaviour
{
    // Usando FixedString64Bytes para serializar o nome do jogador
    private NetworkVariable<FixedString64Bytes> playersName = new NetworkVariable<FixedString64Bytes>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsLocalPlayer)
        {
            // Defina o nome do jogador com base no LobbyManager
            string name = LobbyManager.Instance.GetName() != null ? LobbyManager.Instance.GetName() : $"Player {OwnerClientId}";

            // Atualize o valor da variável de rede com o nome do jogador
            playersName.Value = name;
        }

        // Defina o overlay com o nome sincronizado
        SetOverlay();
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = playersName.Value.ToString();
    }

    void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playersName.Value.ToString()))
        {
            SetOverlay();
            overlaySet = true;
        }
    }
}
