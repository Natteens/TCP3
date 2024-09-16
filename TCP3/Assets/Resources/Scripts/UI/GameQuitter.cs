using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;

public class GameQuitter : MonoBehaviour
{
    public async void QuitToMenu()
    {
        // Tenta remover o jogador do lobby
        try
        {

            if (LobbyService.Instance != null && AuthenticationService.Instance != null)
            {
                // Remove o jogador do lobby de forma assíncrona
                await LobbyService.Instance.RemovePlayerAsync(LobbyManager.Instance.joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                Debug.Log("Player removido do lobby com sucesso.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao remover o jogador do lobby: {e.Message}");
        }

        // Carrega o menu inicial
        await Loader.Load(gameScenes.InitialMenu);
    }
}
