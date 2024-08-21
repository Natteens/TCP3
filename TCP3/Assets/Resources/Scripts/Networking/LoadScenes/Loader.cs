using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum gameScenes
{
    Katalisya, //Cena do jogo
    MainMenu,
    Loading  //Cena de load
}

public static class Loader
{
    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(gameScenes scene)
    {
        // Dizendo pro callback qual cena desejada
        onLoaderCallback = async () =>
        {
           await LoadSceneAsync(scene);
        };

        // Carregando a cena de loading
        SceneManager.LoadScene(gameScenes.Loading.ToString());
    }

    public static async Task LoadSceneAsync(gameScenes scene)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(scene.ToString());
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                onLoaderCallback?.Invoke();
                break;
            }
            await Task.Yield();
        }

        await Task.Delay(1000); 
        loadingOperation.allowSceneActivation = true;
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
