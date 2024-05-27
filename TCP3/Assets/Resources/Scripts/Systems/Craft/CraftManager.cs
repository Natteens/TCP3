using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CraftManager : NetworkBehaviour
{
    [SerializeField] private Managers manager;
    [SerializeField] private List<Recipe> allRecipes;
    public List<Recipe> Recipes { get { return allRecipes; } }

    [SerializeField] private List<GameObject> allRecipesGO;
    public List<GameObject> RecipesGO { get { return allRecipesGO; } set { allRecipesGO = value; } }

    public void Update()
    {
        UpdateRequirements();
    }

    public bool CanCraft(Recipe receita)
    {
        foreach (var requisito in receita.requirements)
        {
            int quantidadeNoInventario = manager.m_player.Inventory.FindAll(item => item == requisito.recurso).Count;
            if (quantidadeNoInventario < requisito.quantidade)
            {
                return false;
            }
        }
        return true;
    }

    public void Craft(Recipe receita)
    {
        if (CanCraft(receita))
        {
            foreach (var requisito in receita.requirements)
            {
                for (int i = 0; i < requisito.quantidade; i++)
                {
                    manager.m_player.Inventory.Remove(requisito.recurso);
                }
            }
            manager.m_player.Inventory.Add(receita.givenItem);
            Debug.Log($"Você criou um(a) {receita.givenItem.ItemName}!");
        }
        else
        {
            Debug.Log("Você não tem os recursos necessários para esta receita.");
        }
    }

    public PlayerManager GetPlayerManager() { return manager.m_player; }

    public void UpdateRequirements()
    {
        if (IsOwner)
        {
            // Itera sobre todos os objetos de receita
            for (int i = 0; i < allRecipesGO.Count; i++)
            {
                // Obtém o componente RecipeUI do objeto atual
                if (allRecipesGO[i] == null) return;

                RecipeUI recipeUI = allRecipesGO[i].GetComponent<RecipeUI>();

                // Itera sobre todos os elementos de UI de requisitos na receita
                foreach (RequirementUI requirementUI in recipeUI.myReqsUI)
                {
                    // Itera sobre todos os requisitos da receita
                    foreach (Requirement requirement in recipeUI.myReqs)
                    {
                        // Atualiza a configuração do UI de requisito com a quantidade do recurso no gerenciador de itens
                        int itemCount = manager.m_player.GetItemCount(requirement.recurso);
                        requirementUI.UpdateConfig(itemCount);
                    }
                }
            }
        } 
    }
}
