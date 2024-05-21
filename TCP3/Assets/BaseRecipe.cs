using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe")]
public class BaseRecipe : ScriptableObject
{
    //Conferir com game designers dps
    public enum RecipeTemplate
    {
        Ore,
        Food,
        Material,
        Weapon
    }

    [Title("Recipe Details")]
    [SerializeField] private string recipeName;
    [SerializeField] private RecipeTemplate type;
    [SerializeField] private Sprite recipeSprite;

    [Title("Recipe given Item")]
    [SerializeField] private BaseItem givenItem;

    [Title("Necessary Resources")]
    [SerializeField] private List<RecipeRequirementSO> requirements;

    public string RecipeName { get { return recipeName; } }
    public Sprite RecipeSprite { get { return recipeSprite; } }
    public RecipeTemplate Type { get { return type; } }
    public List<RecipeRequirementSO> Requirements { get { return requirements; } }
}
