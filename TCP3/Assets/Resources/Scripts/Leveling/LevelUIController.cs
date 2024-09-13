using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    public LevelManager manager;
    private Image xp;
    private TextMeshProUGUI nextLevelText, level, pointsToUp, cons, dest, sobre, sorte, details;
    private int cons_int, dest_int, sobre_int, sorte_int;
    private StatusComponent myStatus;

    private void Start()
    {
        manager.OnXpChanged += UpdateUI;

        xp = GameManager.Instance.xpBar;
        nextLevelText = GameManager.Instance.nextLevelText;
        level = GameManager.Instance.level;
        pointsToUp = GameManager.Instance.pointsToUp;
        cons = GameManager.Instance.cons;
        dest = GameManager.Instance.dest;
        sobre = GameManager.Instance.sobre;
        sorte = GameManager.Instance.sorte;
        details = GameManager.Instance.details;
        myStatus = GetComponent<StatusComponent>();

        cons.text = "";
        sobre.text = "";
        dest.text = "";
        sorte.text = "";

        UpdateUI();
    }

    public void UpdateUI()
    {
        xp.fillAmount = manager.CurrentXp / manager.NextLevelXp;
        nextLevelText.text = $"{manager.CurrentXp} / {manager.NextLevelXp}";
        pointsToUp.text = manager.SkillPoints.ToString();
        level.text = $"Nível {manager.Level}";
        

        details.text = $"Vida: {myStatus.GetStatus(StatusType.Health)} \n" +
                       $"Defesa: {myStatus.GetStatus(StatusType.Defense)} \n" +
                       $"Velocidade de Recarga: {myStatus.GetStatus(StatusType.CooldownReload)} \n" +
                       $"Velocidade de Coleta: {myStatus.GetStatus(StatusType.GatheringSpeed)} \n" +
                       $"Saciedade: {myStatus.GetStatus(StatusType.Satiaty)} \n" +
                       $"Chance de Crítico: {myStatus.GetStatus(StatusType.CritChance)} \n" +
                       $"Chance de Saque: {myStatus.GetStatus(StatusType.LootChance)} \n" +
                       $"Velocidade de Movimento: {myStatus.GetStatus(StatusType.MoveSpeed)}";
    }

}
