using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorUIController : MonoBehaviour
{
    private Image health, stamina, hunger, thirsty;
    [SerializeField] private SurvivalManager survivalManager;
    [SerializeField] private HealthComponent healthManager;

    void Start()
    {
        health = GameManager.Instance.health;
        stamina = GameManager.Instance.stamina;
        hunger = GameManager.Instance.hunger;
        thirsty = GameManager.Instance.thirsty; 
        survivalManager.OnStatusChanged += UpdateUI;
    }

    void OnDestroy()
    {
        if (survivalManager != null)
        {
            survivalManager.OnStatusChanged -= UpdateUI;
        }
    }

    void UpdateUI()
    {
        health.fillAmount = healthManager.CurrentHealth / healthManager.MaxHealth;
        stamina.fillAmount = survivalManager.CurrentStamina / survivalManager.MaxStamina;
        hunger.fillAmount = survivalManager.CurrentHungry / survivalManager.MaxHunger;
        thirsty.fillAmount = survivalManager.CurrentThirsty / survivalManager.MaxThirsty;
    }
}
