using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using StarterAssets;
using Unity.Netcode;


public class GameManager : Singleton<GameManager>
{
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
