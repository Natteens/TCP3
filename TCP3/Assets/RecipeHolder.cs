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
    [SerializeField] private GameObject recipeRequirementPrefab;

    [SerializeField] private Image recipeImage;
    [SerializeField] private TextMeshProUGUI recipeText;

    

    public void Initialize()
    {
        recipeImage.sprite = recipe.RecipeSprite;
        recipeText.text = recipe.RecipeName;

        for (int i = 0; i < recipe.Requirements.Count; i++)
        {
            //recipeR.Initialize();

            GameObject _obj = Instantiate(recipeRequirementPrefab,
            recipeRequirementHolder.transform);

            //_obj.GetComponent<TextMeshProUGUI>().text = recipe.Requirements[i].
        }
           
    }

    public void UpdateRecipe(BaseRecipe _recipe)
    {
        recipe = _recipe;
    }



}
