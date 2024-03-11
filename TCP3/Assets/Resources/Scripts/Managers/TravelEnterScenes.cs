using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
using StarterAssets;

public class TravelEnterScenes : NetworkBehaviour
{
    public LayerMask targetLayer;
    public string dungeonSceneName; // Nome da cena da Dungeon

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        if ((targetLayer.value & 1 << other.gameObject.layer) != 0)
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Chama o método para carregar a cena no servidor e no cliente que interagiu
                LoadSceneForPlayerServerRpc(networkObject.OwnerClientId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadSceneForPlayerServerRpc(ulong clientId)
    {
        // Notifica o cliente específico para mudar para a cena carregada
        SwitchSceneClientRpc(dungeonSceneName, clientId);
    }

    [ClientRpc]
    private void SwitchSceneClientRpc(string sceneName, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Carrega a nova cena de forma aditiva
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

            // Espera até a cena ser carregada para mover o jogador
            StartCoroutine(MovePlayerToScene(clientId, sceneName));
        }
        else
        {
            // Desativa o objeto do jogador para todos os outros jogadores
            SetPlayerVisibility(clientId, false);
        }
    }

    private IEnumerator MovePlayerToScene(ulong clientId, string sceneName)
    {
        // Aguarda até a cena ser carregada
        yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);

        // Move o objeto do jogador para a nova cena
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerNetworkObject != null)
        {
            SceneManager.MoveGameObjectToScene(playerNetworkObject.gameObject, SceneManager.GetSceneByName(sceneName));

            // Chama o método para reinicializar os componentes do jogador
            ThirdPersonController playerController = playerNetworkObject.GetComponent<ThirdPersonController>();
            if (playerController != null)
            {
                playerController.InitializeComponents();
            }
        }

        // Descarrega a cena anterior
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    //Retirar e colocar em um script especifico para isso
    private void SetPlayerVisibility(ulong clientId, bool isVisible)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        if (playerNetworkObject != null)
        {
            // Ativa ou desativa o objeto do jogador dependendo da visibilidade
            playerNetworkObject.gameObject.SetActive(isVisible);
        }
    }
}
