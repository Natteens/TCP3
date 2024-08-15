using UnityEngine;
using Unity.Netcode;
using Mono.CSharp;
using Cinemachine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public Transform aimSpheare;
    public CinemachineVirtualCamera virtualCamera;
    public UI_Inventory uiInventory;
    public UI_Craft uiCraft;
    public GameObject interactMSG;

    public Image health, stamina, hunger, thirsty;

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

}
