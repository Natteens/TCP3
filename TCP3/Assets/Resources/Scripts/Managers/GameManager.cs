using UnityEngine;
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
