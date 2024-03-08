using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    public TextMeshProUGUI playerName;

    // Adicione uma vari�vel para armazenar o nome do jogador
    private string playerDisplayName;

    // M�todo para configurar o nome do jogador
    public void SetPlayerName(string name)
    {
        playerDisplayName = name;
        UpdatePlayerName();
    }

    // M�todo para atualizar o nome do jogador na HUD
    private void UpdatePlayerName()
    {
        if (playerName != null)
        {
            playerName.text = playerDisplayName;
        }
    }
}
