using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum gameScenes
{
    teste,
    Katalisya, //Cena do jogo
    MainMenu,
    InitialMenu,
    Loading  //Cena de load
}

public static class Loader
{
    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static async Task Load(gameScenes scene)
    {
        SceneManager.LoadScene(gameScenes.Loading.ToString());

        await Task.Yield();

        await LoadSceneAsync(scene);
    }

    public static async Task LoadSceneAsync(gameScenes scene)
    {
        try
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
        catch (Exception e)
        {
            Debug.LogError($"Erro ao carregar cena: {e.Message}");
        }
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
