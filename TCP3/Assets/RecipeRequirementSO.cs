using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "RecipeRequiriment")]
public class RecipeRequirementSO : ScriptableObject
{
    [SerializeField] private string needResourceNumber;
    [SerializeField] private BaseItem resource;

    public GameObject prefab;

    public void Initialize()
    {
        prefab.GetComponent<RecipeRequirement>().Initialize(resource,
        needResourceNumber);
    }

}