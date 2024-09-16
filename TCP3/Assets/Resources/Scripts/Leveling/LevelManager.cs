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

        nextLevelXp = 350 + (int)(Mathf.Pow(level, 2f) * 10);
        myStatus = GetComponent<StatusComponent>();

        //Crafts iniciais
        foreach (Craft craft in GameManager.Instance.uiCraft.Level1_4) { GameManager.Instance.uiCraft.GetInventory().AddCraft(craft); }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.L) && GameManager.Instance.isDebugActive) { LevelUp();  }

    }

    public void IncreaseXp(int amount)
    {
        if (currentXp + amount > nextLevelXp) { LevelUp(); }

        currentXp += amount;
        FeedbackManager.Instance.FeedbackText($"+{amount} Exp");
        OnXpChanged.Invoke();
    }

    private void LevelUp()
    {
        FeedbackManager.Instance.FeedbackText("Novo nível alcançado!");
        currentXp = 0;
        level++;
        skillPoints++;
        nextLevelXp = 350 + (int)(Mathf.Pow(level, 2f) * 10);
        OnXpChanged.Invoke(); //COXAS CODE MAS FÉ

        switch (level)
        {
            case 5:
                foreach (Craft craft in GameManager.Instance.uiCraft.Level5_9) { GameManager.Instance.uiCraft.GetInventory().AddCraft(craft); }
                break;

            case 10:
                foreach (Craft craft in GameManager.Instance.uiCraft.Level10_15) { GameManager.Instance.uiCraft.GetInventory().AddCraft(craft); }
                break;
        }
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
