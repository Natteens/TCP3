using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetupManger : MonoBehaviour
{
    private GameObject playerObject;

    private void Awake()
    {
        // Encontra o jogador na nova cena
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            // Chama os m�todos Awake e Start do jogador
            playerObject.GetComponent<ThirdPersonController>().Awake();
            playerObject.GetComponent<ThirdPersonController>().Start();
            playerObject.GetComponent<ThirdPersonController>().CamTargetChecking();
        }
        else
        {
            Debug.LogWarning("Jogador n�o encontrado na nova cena.");
        }
    }
}
