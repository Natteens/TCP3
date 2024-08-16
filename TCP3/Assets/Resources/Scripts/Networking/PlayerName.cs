using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerName : NetworkBehaviour
{
    private NetworkVariable<NetworkString> playersName = new NetworkVariable<NetworkString>();
    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            playersName.Value = $"{LobbyManager.Instance.GetName()}";
        }
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
