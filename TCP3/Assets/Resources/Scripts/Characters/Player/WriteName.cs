using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class WriteName : NetworkBehaviour
{
    private TextMeshProUGUI textname;

    private void Awake()
    {
        if (IsOwner)
        {
            textname = GetComponentInChildren<TextMeshProUGUI>();
            textname.text = LobbyManager.Instance.GetName();
        }
    }
}
