using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeHolder : MonoBehaviour
{

    [SerializeField] private RecipeHead recipeHead;
    [SerializeField] private BaseRecipe recipe;
    
    [SerializeField] private GameObject recipeRequirementHolder;
    [SerializeField] private Image recipeImage;
    [SerializeField] private TextMeshProUGUI recipeText;

    public void Initialize()
    {
        recipeImage.sprite = recipe.RecipeSprite;
        recipeText.text = recipe.RecipeName;

        foreach (RecipeRequirementSO recipeR in recipe.Requirements)
        {
            recipeR.Initialize();

            Instantiate(recipeR.prefab,
            recipeRequirementHolder.transform);
        }
    }

    public void UpdateRecipe(BaseRecipe _recipe)
    {
        recipe = _recipe;
    }



}
