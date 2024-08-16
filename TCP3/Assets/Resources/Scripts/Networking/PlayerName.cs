using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerName : NetworkBehaviour
{
    private string playersName;
    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner && IsLocalPlayer)
        {
            if (LobbyManager.Instance.GetName() != null)
            {
                playersName = $"{LobbyManager.Instance.GetName()}";
            }
            else
            {
                playersName = $"Player {OwnerClientId}";
            }
        }   
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = playersName;
    }


    void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playersName))
        { 
            SetOverlay();
            overlaySet = true;
        }
    }
}
