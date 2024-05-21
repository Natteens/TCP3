using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeHead : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private GameObject contentRecipe;

    public PlayerManager GetManager()
    { 
        return playerManager;
    }

    public void CreateRecipe(BaseRecipe recipe)
    { 
        GameObject _instance = Instantiate(recipePrefab, contentRecipe.transform);
        RecipeHolder _holder = _instance.GetComponent<RecipeHolder>();  

        _holder.UpdateRecipe(recipe);
        _holder.Initialize();
    }
}
