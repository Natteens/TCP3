using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.Netcode;



public class SurvivalManager : NetworkBehaviour
{
    [Range(20, 500)]
    public float MaxStamina = 100, MaxHunger = 100, MaxThirsty = 100, StaminaSpeed = 250;
    public float CurrentStamina, CurrentHungry, CurrentThirsty;
    private bool WhithoutStamina = false;
    private float HungryTimer, ThirstyTimer;
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private StatusComponent statusComponent;
    public event Action OnStatusChanged;
    
    void Start()
    {
        RestaureFull();
        healthComponent.OnRevive += RestaureFull;
    }

    public void RestaureFull()
    {
        CurrentStamina = MaxStamina;
        CurrentHungry = MaxHunger;
        CurrentThirsty = MaxThirsty;
        StatusChanged();
    }

    void Update()
    {
        Stamina();
        Hungry();
        Thirsty();
    }
    void Hungry()
    {
        CurrentHungry -= Time.deltaTime;
        if (CurrentHungry >= MaxHunger)
        {
            CurrentHungry = MaxHunger;
        }
        if (CurrentHungry <= 0)
        {
            CurrentHungry = 0;
            HungryTimer += Time.deltaTime;
            if (HungryTimer >= 3)
            {
                healthComponent?.TakeDamage(healthComponent.MaxHealth * 0.005f);
                CurrentStamina -= (MaxStamina * 0.1f);
                HungryTimer = 0;
            }
        }
        else
        {
            HungryTimer = 0;
        }
        StatusChanged();
    }
    void Thirsty()
    {
        CurrentThirsty -= (1 - (statusComponent.GetStatus(StatusType.Satiaty) / 100f)) * CurrentThirsty;
        if (CurrentThirsty >= MaxThirsty)
        {
            CurrentThirsty = MaxThirsty;
        }
        if (CurrentThirsty <= 0)
        {
            CurrentThirsty = 0;
            ThirstyTimer += Time.deltaTime;
            if (ThirstyTimer >= 3)
            {
                healthComponent?.TakeDamage(healthComponent.MaxHealth * 0.005f);
                CurrentStamina -= (MaxStamina * 0.1f);
                ThirstyTimer = 0;
            }
        }
        else
        {
            ThirstyTimer = 0;
        }
        StatusChanged();
    }
    void Stamina()
    {
        float multEuler = ((1 / MaxStamina) * CurrentStamina) * ((1 / MaxHunger) * CurrentHungry);
        if (CurrentStamina >= MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }
        else
        {
            CurrentStamina += Time.deltaTime * (StaminaSpeed / 40) * Mathf.Pow(2.718f, multEuler);
        }
        if (CurrentStamina <= 0)
        {
            CurrentStamina = 0;
            WhithoutStamina = true;
        }
        if (WhithoutStamina && CurrentStamina >= (MaxStamina * 0.15f))
        {
            WhithoutStamina = false;
        }
        StatusChanged();
    }  
    void StatusChanged()
    {
        OnStatusChanged?.Invoke();
    }

    public bool UseStamina()
    {
        float multEuler = ((1 / MaxStamina) * CurrentStamina) * ((1 / MaxHunger) * CurrentHungry);
        if (!WhithoutStamina)
        {
            CurrentStamina -= Time.deltaTime * (StaminaSpeed / 15) * Mathf.Pow(2.718f, multEuler);
            return true;
        }
        return false;
    }


    public void IncreaseStats(Consumable consumable)
    {
        switch (consumable.type)
        {
            case ConsumableType.Fome:
                CurrentHungry += consumable.restoreAmount;
                break;
            case ConsumableType.Sede:
                CurrentThirsty += consumable.restoreAmount;
                break;
            case ConsumableType.Energia:
                CurrentStamina += consumable.restoreAmount;
                break;
        }
        Debug.Log("Foi");
    }

}

