using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameQuitter : MonoBehaviour
{
    private GameObject[] allPlayers;
    public float searchInterval = 0.1f;  // Tempo em segundos entre as buscas

    // Método de busca de jogadores
    private void FindPlayers()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
    }

    public async void QuitToMenu()
    {
        // Tenta remover o jogador do lobby
        try
        {
            // Chama a função de busca diretamente
            FindPlayers();

            // Agora usa a lista 'allPlayers' atualizada
            if (allPlayers != null && allPlayers.Length > 0)
            {
                foreach (GameObject player in allPlayers)
                {
                    if (player.GetComponent<PlayerInput>().enabled == true)
                    {
                        LobbyManager.Instance.LeaveLobby();
                        // Carrega o menu inicial
                        await Loader.Load(gameScenes.InitialMenu);
                        return;
                    }
                }

                Debug.LogWarning("Nenhum jogador correspondente encontrado.");
            }
            else
            {
                Debug.LogWarning("Nenhum jogador encontrado.");
            }
                
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao remover o jogador do lobby: {e.Message}");
        }

    }

}
