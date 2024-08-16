using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Necessário para FixedString

public class PlayerName : NetworkBehaviour
{
    private NetworkVariable<FixedString64Bytes> playersName = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server);

    private bool overlaySet = false;

    private TextMeshProUGUI localPlayerOverlay;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsLocalPlayer)
        {
            // Solicita ao servidor para definir o nome do jogador
            SetPlayerNameServerRpc(LobbyManager.Instance.GetName() ?? $"Player {OwnerClientId}");
        }

        // Defina o overlay com o nome sincronizado
        SetOverlay();
    }

    [ServerRpc]
    private void SetPlayerNameServerRpc(string name)
    {
        playersName.Value = name;
    }

    public void SetOverlay()
    {
        localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (localPlayerOverlay != null)
        {
            localPlayerOverlay.text = playersName.Value.ToString();
        }
    }

    void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playersName.Value.ToString()))
        {
            SetOverlay();
            overlaySet = true;
        }
    }

    void LateUpdate()
    {
       transform.rotation = Camera.main.transform.rotation;   
    }
}
