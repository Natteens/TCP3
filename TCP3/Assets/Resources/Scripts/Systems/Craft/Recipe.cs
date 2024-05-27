using UnityEngine;
using static BaseItem;

[CreateAssetMenu(fileName = "NovaReceita", menuName = "Itens/Recipe")]
public class Recipe : ScriptableObject
{
    public enum RecipeType
    {
        Ore,
        Food,
        Material,
        Weapon
    }

    public string recipeName;
    [SerializeField] private RecipeType type;
    [HideInInspector] public RecipeType Type { get { return type; } }

    public BaseItem givenItem;
    public Requirement[] requirements;
}
