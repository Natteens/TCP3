using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;
    public UI_Inventory uiInventory;
    public UI_Craft uiCraft;
    public GameObject interactMSG;
    public GameObject HarvestHolder;

    public Image health, stamina, hunger, thirsty;

    [Header("Day/Night Cycle")]
    public bool isNight;

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
