using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class RecipeRequirement : MonoBehaviour
{

    [SerializeField] private string needResourceNumber;
    [SerializeField] private BaseItem resource;
    [SerializeField] private TextMeshProUGUI needResourceText;
    [SerializeField] private Image needResourceImage;

    public void Initialize(BaseItem _ITEM, string _ResourceNumber)
    { 
        needResourceNumber = _ResourceNumber;
        resource = _ITEM;
    }
    public void Start()
    {
        needResourceImage.sprite = resource.ItemSprite;
    }

    public void UpdateNeedResourceNumber(int actualResource)
    { 
        needResourceText.text = actualResource.ToString() + "/" + needResourceNumber;
    }
}

