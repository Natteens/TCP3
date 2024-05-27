using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequirementUI : MonoBehaviour
{
    public Image icone;
    public TextMeshProUGUI nome;
    private int requirementQuantity;

    public void Configurar(Requirement requirement, int currentResourceQuantity)
    {
        if (requirement != null)
        {
            icone.sprite = requirement.recurso.ItemSprite;
            requirementQuantity = requirement.quantidade;
            nome.text = currentResourceQuantity.ToString() + "/" + requirement.quantidade.ToString();
        } 
    }

    public void UpdateConfig(int currentResourceQuantity)
    {
        nome.text = currentResourceQuantity.ToString() + "/" + requirementQuantity.ToString();
    }
}
