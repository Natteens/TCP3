using UnityEngine;
using Unity.Netcode;
using Mono.CSharp;
using Cinemachine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;
    public UI_Inventory uiInventory;
    public UI_Craft uiCraft;
    public GameObject interactMSG;

    public Image health, stamina, hunger, thirsty;

    private NetworkVariable<int> joinedPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private NetworkVariable<int> maxPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> hasAssigned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private List<NetworkClient> playerList;


    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Start()
    {
        maxPlayers.Value = LobbyManager.Instance.GetJoinedLobby().MaxPlayers;

        if (IsServer)
        {
            playerList = (List<NetworkClient>)NetworkManager.Singleton.ConnectedClientsList;
        }

    }

    private void Update()
    {
        if (!IsServer) return;
        joinedPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
// adicionei o start e o update pra tester se precisarmos ja tao ai 
