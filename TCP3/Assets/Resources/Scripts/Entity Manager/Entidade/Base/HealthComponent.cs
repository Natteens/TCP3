﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;
using System.Linq;

public class HealthComponent : NetworkBehaviour, IHealth
{

    [field: SerializeField] private NetworkVariable<float> currentHealth = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [field: SerializeField] private NetworkVariable<float> maxHealth = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    public float CurrentHealth
    {
        get => currentHealth.Value;
        private set => currentHealth.Value = value;
    }

    [SerializeField]
    public float MaxHealth
    {
        get => maxHealth.Value;
        private set => maxHealth.Value = value;
    }

    [SerializeField] public bool IsAlive => CurrentHealth > 0;

    public event Action OnDeath;
    public event Action OnRevive;
    public event Action<float> OnTakeDamage;
    public event Action<float> OnHeal;

    private Action<float> applyDamageAction;
    private Action<float> applyHealAction;

    private void Awake()
    {
        applyDamageAction += TakeDamage;
        applyHealAction += Heal;
    }

    private void Start()
    {
        InitializeHealth();

        // Monitorando mudanças na CurrentHealth
        currentHealth.OnValueChanged += (previousValue, newValue) =>
        {
            if (newValue <= 0 && previousValue > 0)
            {
                Die();
            }
        };
    }

    public void InitializeHealth()
    {
        StatusComponent statusComponent = GetComponent<StatusComponent>();
        statusComponent.OnStatusChanged += HandleStatusChanged;
        statusComponent.OnEffectApplied += HandleEffectApplied;
        MaxHealth = statusComponent.GetStatus(StatusType.Health);
        CurrentHealth = MaxHealth; // Atribui o valor inicial usando a propriedade
    }

    public bool IsOnDeathSubscribed(Action action)
    {
        return OnDeath != null && OnDeath.GetInvocationList().Contains(action);
    }

    private void HandleStatusChanged(Dictionary<StatusType, float> currentStatus)
    {
        if (currentStatus.TryGetValue(StatusType.Health, out var health))
        {
            MaxHealth = health;
            CurrentHealth = (CurrentHealth >= MaxHealth) ? MaxHealth : CurrentHealth;
        }
    }

    private void HandleEffectApplied(StatusEffectData effect)
    {
        if (effect.statusType == StatusType.None)
        {
            var action = effect.isBuff ? applyHealAction : applyDamageAction;
            action?.Invoke(effect.effectValue);
        }
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        CurrentHealth -= amount; // Modifica usando a propriedade
        OnTakeDamage?.Invoke(amount);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;
        CurrentHealth += amount; // Modifica usando a propriedade
        CurrentHealth = (CurrentHealth >= MaxHealth) ? MaxHealth : CurrentHealth;
        OnHeal?.Invoke(amount);
        Debug.Log("Recebendo cura " + amount);
    }

    public void Die()
    {
        CurrentHealth = 0;

        //Vai ficar assim e fé!
        if (gameObject.CompareTag("Player"))
        {
            Inventory inventory = GameManager.Instance.uiInventory.GetInventory();
            int amountToDrop = UnityEngine.Random.Range(0, inventory.GetItemList().Count);

            for (int i = 0; i < amountToDrop; i++)
            {
                inventory.DropRandomItem();
            }

            Revive();
            return;
        }

        OnDeath?.Invoke();
    }

    public void Revive()
    {
        if (IsAlive) return;
        CurrentHealth = MaxHealth;

        //Vai ficar assim e fé!
        GameManager.Instance.GoToRespawnPoint(transform);
        OnRevive?.Invoke();
    }
}
