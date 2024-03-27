using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {


    public static LobbyCreateUI Instance { get; private set; }


    [SerializeField] private Button createButton;
    [SerializeField] private Button lobbyNameButton;
    [SerializeField] private Button publicPrivateButton;
    [SerializeField] private Button maxPlayersButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI publicPrivateText;
    [SerializeField] private TextMeshProUGUI maxPlayersText;


    private string lobbyName;
    private bool isPrivate;
    private int maxPlayers;

    private void Awake() 
    {
        Instance = this;

        createButton.onClick.AddListener(() => {
            LobbyManager.Instance.CreateLobby(
                lobbyName,
                maxPlayers,
                isPrivate
            );
            Hide();
        });


        lobbyNameButton.onClick.AddListener(() => {
            UI_InputWindow.Show_Static("Nome da Sala", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .123456789,-", 20,
            () => {
                // Cancel
            },
            (string lobbyName) => {
                this.lobbyName = lobbyName;
                UpdateText();
            });
        });

        publicPrivateButton.onClick.AddListener(() => {
            isPrivate = !isPrivate;
            UpdateText();
        });

        maxPlayersButton.onClick.AddListener(() => {
            UI_InputWindow.Show_Static("Jogadores", maxPlayers,
            () => {
                // Cancel
            },
            (int maxPlayers) => {
                this.maxPlayers = maxPlayers;
                UpdateText();
            });
        });

        Hide();
    }

    public void UpdateText()
    {
        lobbyNameText.text = lobbyName;
        publicPrivateText.text = isPrivate ? "Privado" : "Publico";
        maxPlayersText.text = maxPlayers.ToString();
    }

    public void Hide()
    {
        //Debug.Log("Rodei hide lobbycreateUI");

        gameObject.SetActive(false);
    }

    public void Show() {

        //Debug.Log("Rodei show lobby create UI");

        gameObject.SetActive(true);
        lobbyName = "MinhaSala";
        isPrivate = false;
        maxPlayers = 4;

        UpdateText();
    }

}