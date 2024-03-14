using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class TravelEnterScenes : NetworkBehaviour
{
    public LayerMask targetLayer;
    public string dungeonSceneName; // Nome da cena da Dungeon

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && (targetLayer.value & 1 << other.gameObject.layer) != 0)
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Chama o método para carregar a cena no servidor e no cliente que interagiu
                LoadSceneForPlayerServerRpc(networkObject.OwnerClientId, dungeonSceneName);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadSceneForPlayerServerRpc(ulong clientId, string sceneName)
    {
        // Notifica todos os clientes para mudar para a cena carregada
        SwitchSceneClientRpc(sceneName, clientId);
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
    }

    private IEnumerator MovePlayerToScene(ulong clientId, string sceneName)
    {
        // Aguarda até a cena ser carregada
        yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);

        // Move o objeto do jogador para a nova cena
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
        SceneManager.MoveGameObjectToScene(playerNetworkObject.gameObject, SceneManager.GetSceneByName(sceneName));
        // Descarrega a cena anterior
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}
