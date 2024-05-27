using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeUI : MonoBehaviour
{
    public Image icone;
    public TextMeshProUGUI nome;

    public GameObject requirementHolder;
    public GameObject requirementPrefab;

    [HideInInspector] public List<RequirementUI> myReqsUI;
    [HideInInspector] public List<Requirement> myReqs;

    [HideInInspector] public PlayerManager pManager; 

    public void Configurar(Recipe recipe)
    {
        if (recipe != null)
        {
            icone.sprite = recipe.givenItem.ItemSprite;
            nome.text = recipe.recipeName;
            InstantiateRequirements(recipe.requirements);
        }
    }

    private void InstantiateRequirements(Requirement[] requirements)
    { 
        foreach (Requirement r in requirements) 
        {
            GameObject instance = Instantiate(requirementPrefab, requirementHolder.transform);
            RequirementUI _instanceUI = instance.GetComponent<RequirementUI>();

            _instanceUI.Configurar(r, pManager.GetItemCount(r.recurso));
            myReqsUI.Add(_instanceUI);
            myReqs.Add(r);

        }
    }
}
