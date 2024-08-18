using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;


public class HealthComponent : NetworkBehaviour, IHealth
{
    [SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }
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
    }

    private void InitializeHealth()
    {
        StatusComponent statusComponent = GetComponent<StatusComponent>();
        statusComponent.OnStatusChanged += HandleStatusChanged;
        statusComponent.OnEffectApplied += HandleEffectApplied;
        MaxHealth = statusComponent.GetStatus(StatusType.Health);
        CurrentHealth = MaxHealth;
    }

    private void HandleStatusChanged(Dictionary<StatusType, float> currentStatus)
    {
        if (!IsOwner) return;
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
        if (!IsAlive || !IsOwner) return;
        CurrentHealth -= amount;
        OnTakeDamage?.Invoke(amount);
        CurrentHealth = (CurrentHealth <= 0) ? 0 : CurrentHealth;
        if ((CurrentHealth = (CurrentHealth <= 0) ? 0 : CurrentHealth) <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (!IsAlive || !IsOwner) return;
        CurrentHealth += amount;
        CurrentHealth = (CurrentHealth >= MaxHealth) ? MaxHealth : CurrentHealth;
        OnHeal?.Invoke(amount);
        Debug.Log("Recebendo cura " + amount);
    }

    public void Die()
    {
        if (!IsAlive || !IsOwner) return;
        OnDeath?.Invoke();
        CurrentHealth = (CurrentHealth <= 0) ? 0 : CurrentHealth;
    }

    public void Revive()
    {
        if (!IsAlive || !IsOwner) return;
        CurrentHealth = MaxHealth;
        OnRevive?.Invoke();
    }
}
