using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UXUIManager : NetworkBehaviour
{
    public PlayerManager playerManager;
    public TextMeshProUGUI constitutionText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI precisionText;
    public TextMeshProUGUI luckText;

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
       constitutionText.text = $"Constituição: {playerManager.StatusBase.status.constitution}";
       strengthText.text = $"Força:  {playerManager.StatusBase.status.strength}";
       agilityText.text = $"Agilidade: {playerManager.StatusBase.status.agility}";
       precisionText.text = $"Precisão: {playerManager.StatusBase.status.precision}";
       luckText.text = $"Sorte: {playerManager.StatusBase.status.luck}";     
    }

    public void UpgradeStats(int index)
    {
        int currentLevel = GetStatusLevel(index);
        int upgradeCost = currentLevel;

        // Verifica se o jogador tem pontos de habilidade suficientes
        if (playerManager.xpTracker.SkillPoints >= upgradeCost && currentLevel < 6)
        {
            // Decrementa os pontos de habilidade
            playerManager.xpTracker.SkillPoints -= upgradeCost;

            // Incrementa o nível da habilidade
            IncrementStatusLevel(index);
        }
    }

    public void DowngradeStats(int index)
    {
        int currentLevel = GetStatusLevel(index);

        // Verifica se o nível atual da habilidade é maior que 1
        if (currentLevel > 1)
        {
            // Calcula o custo total para atualizar a habilidade para o nível atual
            int totalUpgradeCost = (currentLevel * (currentLevel - 1)) / 2;

            Debug.Log($"Downgrading skill {index}. Current level: {currentLevel}, Total upgrade cost: {totalUpgradeCost}, Current skill points: {playerManager.xpTracker.SkillPoints}");

            // Incrementa os pontos de habilidade
            playerManager.xpTracker.SkillPoints += totalUpgradeCost;

            Debug.Log($"After refund, skill points: {playerManager.xpTracker.SkillPoints}");

            // Decrementa o nível da habilidade
            DecrementStatusLevel(index);
        }
    }



    private int GetStatusLevel(int Index)
    {
        switch (Index)
        {
            case 0:
                return playerManager.StatusBase.status.constitution;
            case 1:
                return playerManager.StatusBase.status.strength;
            case 2:
                return playerManager.StatusBase.status.agility;
            case 3:
                return playerManager.StatusBase.status.precision;
            case 4:
                return playerManager.StatusBase.status.luck;
            default:
                return 0;
        }
    }

    private void IncrementStatusLevel(int Index)
    {
        switch (Index)
        {
            case 0:
                playerManager.StatusBase.status.constitution++;
                break;
            case 1:
                playerManager.StatusBase.status.strength++;
                break;
            case 2:
                playerManager.StatusBase.status.agility++;
                break;
            case 3:
                playerManager.StatusBase.status.precision++;
                break;
            case 4:
                playerManager.StatusBase.status.luck++;
                break;
        }
    }

    private void DecrementStatusLevel(int Index)
    {
        switch (Index)
        {
            case 0:
                playerManager.StatusBase.status.constitution--;
                break;
            case 1:
                playerManager.StatusBase.status.strength--;
                break;
            case 2:
                playerManager.StatusBase.status.agility--;
                break;
            case 3:
                playerManager.StatusBase.status.precision--;
                break;
            case 4:
                playerManager.StatusBase.status.luck--;
                break;
        }

    }


}
