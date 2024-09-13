using System;
using Unity.Netcode;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int nextLevelXp;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int skillPoints = 0;
    [SerializeField] private StatusComponent myStatus;

    public int CurrentXp { get { return currentXp; } }
    public int Level { get { return level; } }
    public int NextLevelXp { get { return nextLevelXp; } }
    public int SkillPoints { get { return skillPoints; } set { skillPoints = value; } }

    public event Action OnXpChanged;

    private void Start()
    {
        if (!IsOwner) return;

        nextLevelXp = 150 + (int)(Mathf.Pow(level, 2f) * 10);
        myStatus = GetComponent<StatusComponent>();
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

    public void ApplyStatusChanges(int consPoints, int destPoints, int sobrePoints, int sortePoints)
    {
        // Aplica os pontos aos status
        myStatus.ApplyConstitution(consPoints);
        myStatus.ApplySurvival(sobrePoints);
        myStatus.ApplyDexterity(destPoints);
        myStatus.ApplyLuck(sortePoints);
    }
}
