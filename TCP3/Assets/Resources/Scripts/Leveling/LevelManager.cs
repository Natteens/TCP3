using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int nextLevelXp;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int skillPoints = 0;

    public int CurrentXp { get { return currentXp; } }
    public int Level { get { return level; } }
    public int NextLevelXp { get { return nextLevelXp; } }
    public int SkillPoints { get { return skillPoints; } }

    public event Action OnXpChanged;
    private void Start()
    {
        if (!IsOwner) return;

        nextLevelXp = 150 + (int)(Mathf.Pow(level, 2f) * 10);
       

    }

    public void IncreaseXp(int amount)
    {
        if (currentXp + amount > nextLevelXp) { LevelUp(); }

        currentXp += amount;
        OnXpChanged.Invoke();
    }

    private void LevelUp()
    {
        currentXp = 0;
        level++;
        skillPoints++;
        nextLevelXp = 150 + (int)(Mathf.Pow(level, 2f) * 10);
    }

    
}
