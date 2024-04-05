using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button startGameLobbyButton;


    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);


        leaveLobbyButton.onClick.AddListener(() => { LobbyManager.Instance.LeaveLobby(); });
        startGameLobbyButton.onClick.AddListener(() => { LobbyManager.Instance.StartGame(); });
    }

    private void Start() {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;

        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) 
        {
            if (playerSingleTemplate != null && container != null)
            {
                Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
                playerSingleTransform.gameObject.SetActive(true);
                LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

                lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                    LobbyManager.Instance.IsLobbyHost() &&
                    player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
                );

                lobbyPlayerSingleUI.UpdatePlayer(player);
            }    
        }

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;

        Show();
    }

    private void ClearLobby()
    {
        if (container != null)
        {
            foreach (Transform child in container)
            {
                if (child == playerSingleTemplate) continue;

                if (child.gameObject != null) Destroy(child.gameObject);

            }
        } 
    }

    private void Hide() 
    {
        if(gameObject != null)
        gameObject.SetActive(false);
        //Debug.Log("rodei hide lobbyUI");
    }

    private void Show() 
    {
        if (gameObject != null)
        gameObject.SetActive(true);
        //Debug.Log("rodei show lobbyUI");
    }

    public void ControlStartButton(bool set)
    { 
        startGameLobbyButton.gameObject.SetActive(set);
    }

}