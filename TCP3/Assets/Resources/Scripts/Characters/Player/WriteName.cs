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

    private void Awake()
    {
        textname = GetComponentInChildren<TextMeshProUGUI>();
         
    }

    private void Start()
    {
        if (IsServer)
        {
            SetPlayerNameClientRpc(LobbyManager.Instance.GetName());
        }
        
            
    }

    [ClientRpc]
    private void SetPlayerNameClientRpc(string playername)
    {
        textname.text = playername;
    }
}
