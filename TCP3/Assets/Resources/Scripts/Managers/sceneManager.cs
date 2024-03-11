using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public string gameplaySceneName = "Nathan";

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
    }


}
