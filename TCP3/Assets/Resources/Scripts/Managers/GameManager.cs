using UnityEngine;
using Unity.Netcode;
using Mono.CSharp;
using Cinemachine;


public class GameManager : Singleton<GameManager>
{
    public CinemachineVirtualCamera virtualCamera;
    public UI_Inventory uiInventory;
    public UI_Craft uiCraft;

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
