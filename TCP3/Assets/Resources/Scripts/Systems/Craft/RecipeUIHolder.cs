using UnityEngine;

public class RecipeUIHolder : MonoBehaviour
{
    [SerializeField] private Managers manager;

    public GameObject recipePrefab;

    public void AdicionarReceita(Recipe _recipe)
    {
        GameObject newRecipe = Instantiate(recipePrefab, transform);
        RecipeUI _recipeUI = newRecipe.GetComponent<RecipeUI>();
        _recipeUI.pManager = manager.m_player;

        _recipeUI.Configurar(_recipe);

        manager.m_craft.Recipes.Add(_recipe);
        manager.m_craft.RecipesGO.Add(newRecipe);
    }

    public void Update()
    {
        DebugAddRecipe();
    }

    public void DebugAddRecipe()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            AdicionarReceita(manager.m_craft.Recipes[0]);
        }
    }
}
