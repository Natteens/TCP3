using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public string gameplaySceneName = "Nathan"; // Nome da cena de gameplay

    private bool playersSpawned = false; // Flag para controlar se os jogadores já foram instanciados ou não

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verifica se a cena carregada é a cena de gameplay e se os jogadores ainda não foram instanciados
        if (scene.name == "Nathan" && !playersSpawned)
        {
            // Instancia os jogadores na cena de gameplay
            SpawnPlayers();

            // Define a flag para indicar que os jogadores foram instanciados
            playersSpawned = true;
        }
        else if (scene.name != "Nathan")
        {
            // Se a cena carregada não for a cena de gameplay, limpa a flag para permitir o respawn dos jogadores
            playersSpawned = false;
        }
    }

    public void SpawnPlayers()
    {
        // Verifica se o jogo já foi iniciado antes de instanciar os jogadores
        if (!LobbyManager.Instance.startedGame)
        {
            Debug.Log("O jogo ainda não foi iniciado. Aguardando...");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned.");
            return;
        }

        foreach (var player in LobbyManager.Instance.joinnedLobby.Players)
        {
            // Spawna cada jogador na cena de gameplay
            string playerName = player.Data["name"].Value;
            Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

            // Instancia o prefab do jogador
            GameObject playerGameObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            if (playerGameObject == null)
            {
                Debug.LogError("Failed to instantiate player prefab.");
                continue; // Pula para o próximo jogador
            }

            // Obtém o componente PlayerSettings do jogador
            PlayerSettings playerSettings = playerGameObject.GetComponent<PlayerSettings>();

            if (playerSettings == null)
            {
                Debug.LogError("PlayerSettings component not found on player prefab.");
                continue; // Pula para o próximo jogador
            }

            // Se o jogador tiver um PlayerSettings, define o nome do jogador
            playerSettings.SetPlayerName(playerName);

            // Obtém o NetworkObject do jogador
            NetworkObject networkObject = playerGameObject.GetComponent<NetworkObject>();

            // Se o jogador tiver um NetworkObject, faça o spawn dele
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            else
            {
                Debug.LogWarning("NetworkObject component not found on player prefab.");
            }
        }
    }

}
