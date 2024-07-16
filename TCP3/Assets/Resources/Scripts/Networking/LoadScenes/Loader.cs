using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }

    public enum Scene
    { 
        Nathan, //Cena do jogo
        CenaTesteNetwork, //Cena teste
        MainMenu,
        Loading  //Cena de load
    }

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene)
    {
        
        //Dizendo pro meu callback qual cena desejada
        onLoaderCallback = () =>
        {
            //Ten que usar gameobj pq nao da pra startar corotina sem o monobehaviour!
            GameObject loadingGameObject = new GameObject("Loading GameOBJ");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };  

        //Carregando a cena de loading 
        SceneManager.LoadScene(Scene.Loading.ToString());

    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null) { return loadingAsyncOperation.progress; }
        else { return 1f; }
    }

    public static void LoaderCallback()
    {
        //trigga depois do primeiro tick de update 
        //Executando o loadercallback que vai carregar a cena desejada
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        } 
    }
}
