using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public string gameplaySceneName = "Nathan";

    private static sceneManager _instance;
    public static sceneManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameplayScene());
    }

    private IEnumerator LoadGameplayScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameplaySceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

       // SpawnPlayer();
    }


}
