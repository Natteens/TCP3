﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Globalization;
using Unity.Netcode;

public class StatusComponent : NetworkBehaviour
{
    [SerializeField] private EntityStatus baseStatus;
    [SerializeField] private List<StatusEffectData> activeEffects = new List<StatusEffectData>();

    public Dictionary<StatusType, float> currentStatus = new Dictionary<StatusType, float>();

    public event Action<Dictionary<StatusType, float>> OnStatusChanged;
    public event Action<List<StatusEffectData>> OnEffectsUpdated;
    public event Action<StatusEffectData> OnEffectApplied;
    public event Action<StatusEffectData> OnEffectRemoved;

    //fazer RPC pra esse script dps

    private void Awake()
    {
        InitializeStatus();
    }

    private void Update()
    {
        UpdateEffects();
    }

    private void InitializeStatus()
    {
        currentStatus.Clear();
        foreach (var baseStat in baseStatus.baseStats)
        {
            currentStatus[baseStat.statusType] = baseStat.value;
        }
        OnStatusChanged?.Invoke(currentStatus);
    }

    public float GetStatus(StatusType type)
    {
        return currentStatus.ContainsKey(type) ? currentStatus[type] : 0;
    }

    public void ApplyEffect(StatusEffectData effect)
    {
        effect.startTime = Time.time;
        activeEffects.Add(effect);
        ApplyImmediateEffect(effect);
        OnEffectApplied?.Invoke(effect);
        OnEffectsUpdated?.Invoke(activeEffects);
    }

    public void RemoveEffect(StatusEffectData effect)
    {
        if (activeEffects.Contains(effect))
        {
            RevertEffect(effect);
            activeEffects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
            OnEffectsUpdated?.Invoke(activeEffects);
        }
    }

    public void RemovePermanentEffect(StatusEffectData effect)
    {
        if (effect.isPermanent && activeEffects.Contains(effect))
        {
            RevertEffect(effect);
            activeEffects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
            OnEffectsUpdated?.Invoke(activeEffects);
        }
    }

    private void UpdateEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StatusEffectData effect = activeEffects[i];
            effect.UpdateEffect(Time.deltaTime);

            if (effect.isContinuous && effect.elapsedTime >= effect.interval)
            {
                ApplyContinuousEffect(effect);
                effect.elapsedTime = 0;
            }

            if (effect.IsExpired)
            {
                RemoveEffect(effect);
            }
        }
    }

    private void ApplyImmediateEffect(StatusEffectData effect)
    {
        ApplyModifier(effect.statusType, effect.effectValue * (effect.isBuff ? 1 : -1));

        if (effect.effectPrefab != null)
        {
            GameObject particleInstance = Instantiate(effect.effectPrefab, transform.position, Quaternion.identity, transform);
            Destroy(particleInstance, effect.duration);
        }
    }

    private void ApplyContinuousEffect(StatusEffectData effect)
    {
        if (effect.elapsedTime >= effect.interval)
        {
            if (effect.statusType == StatusType.None)
            {
                OnEffectApplied?.Invoke(effect);
            }
            else
            {
                ApplyModifier(effect.statusType, effect.effectValue * (effect.isBuff ? 1 : -1));
            }
            effect.elapsedTime = 0; 
        }
    }

    public void UpdateStatus(Dictionary<StatusType, float> newStatus)
    {
        currentStatus = newStatus;
        OnStatusChanged?.Invoke(currentStatus);
    }

    private void ApplyModifier(StatusType type, float value)
    {
        if (currentStatus.ContainsKey(type))
        {
            currentStatus[type] += value;
        }
        else
        {
            currentStatus[type] = value;
        }
        OnStatusChanged?.Invoke(currentStatus);
    }

    private void RevertEffect(StatusEffectData effect)
    {
        ApplyModifier(effect.statusType, effect.effectValue * (effect.isBuff ? -1 : 1));
    }

    public void ApplyConstitution(int points)
    {
        ApplyModifier(StatusType.Health, points);
        ApplyModifier(StatusType.Defense, points);
    }

    public void ApplySurvival(int points)
    {
        ApplyModifier(StatusType.GatheringSpeed, points);
        ApplyModifier(StatusType.Satiaty, points);
    }

    public void ApplyDexterity(int points)
    {
        ApplyModifier(StatusType.CooldownAbility, points);
        ApplyModifier(StatusType.CooldownReload, points);
    }

    public void ApplyLuck(int points)
    {
        ApplyModifier(StatusType.CritChance, points);
        ApplyModifier(StatusType.LootChance, points);
    }
}
