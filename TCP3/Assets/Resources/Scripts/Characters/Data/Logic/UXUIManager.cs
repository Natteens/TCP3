using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UXUIManager : MonoBehaviour
{
    [SerializeField] private Managers manager;
    [SerializeField] private TextMeshProUGUI constitutionText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI agilityText;
    [SerializeField] private TextMeshProUGUI precisionText;
    [SerializeField] private TextMeshProUGUI luckText;

    //private Popup popup;
    [SerializeField] private RectTransform collectedFeedback;
    [SerializeField] private Vector2 originalPosFeedback;


    private void Start()
    {
        collectedFeedback.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        originalPosFeedback = collectedFeedback.anchoredPosition;
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
       constitutionText.text = $"Constituição: {manager.m_player.StatusBase.status.constitution}";
       strengthText.text = $"Força:  {manager.m_player.StatusBase.status.strength}";
       agilityText.text = $"Agilidade: {manager.m_player.StatusBase.status.agility}";
       precisionText.text = $"Precisão: {manager.m_player.StatusBase.status.precision}";
       luckText.text = $"Sorte: {manager.m_player.StatusBase.status.luck}";     
    }

    public void UpgradeStats(int index)
    {
        int currentLevel = GetStatusLevel(index);
        int upgradeCost = currentLevel;

        // Verifica se o jogador tem pontos de habilidade suficientes
        if (manager.m_player.xpTracker.SkillPoints >= upgradeCost && currentLevel < 6)
        {
            // Decrementa os pontos de habilidade
            manager.m_player.xpTracker.SkillPoints -= upgradeCost;

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
            // Incrementa os pontos de habilidade
            manager.m_player.xpTracker.SkillPoints += currentLevel - 1;

            // Decrementa o nível da habilidade
            DecrementStatusLevel(index);
        }
    }

    private int GetStatusLevel(int Index)
    {
        switch (Index)
        {
            case 0:
                return manager.m_player.StatusBase.status.constitution;
            case 1:
                return manager.m_player.StatusBase.status.strength;
            case 2:
                return manager.m_player.StatusBase.status.agility;
            case 3:
                return manager.m_player.StatusBase.status.precision;
            case 4:
                return manager.m_player.StatusBase.status.luck;
            default:
                return 0;
        }
    }

    private void IncrementStatusLevel(int Index)
    {
        switch (Index)
        {
            case 0:
                manager.m_player.StatusBase.status.constitution++;
                break;
            case 1:
                manager.m_player.StatusBase.status.strength++;
                break;
            case 2:
                manager.m_player.StatusBase.status.agility++;
                break;
            case 3:
                manager.m_player.StatusBase.status.precision++;
                break;
            case 4:
                manager.m_player.StatusBase.status.luck++;
                break;
        }
    }

    private void DecrementStatusLevel(int Index)
    {
        switch (Index)
        {
            case 0:
                manager.m_player.StatusBase.status.constitution--;
                break;
            case 1:
                manager.m_player.StatusBase.status.strength--;
                break;
            case 2:
                manager.m_player.StatusBase.status.agility--;
                break;
            case 3:
                manager.m_player.StatusBase.status.precision--;
                break;
            case 4:
                manager.m_player.StatusBase.status.luck--;
                break;
        }

    }

    void ShowTipMsg(string msg)
    { 
        //popup.ShowPopup(msg);
    }

    IEnumerator CollectedFeedbackMsg(string msg)
    {
        TextMeshProUGUI feedbackText = collectedFeedback.GetComponent<TextMeshProUGUI>();

        feedbackText.text = msg;

        feedbackText.color = new Color(1, 1, 1, 1);

        for (float t = 0; t < 1.01f; t+= Time.deltaTime/1.5f)
        {
            collectedFeedback.anchoredPosition += Vector2.up * 25 * Time.deltaTime;
            feedbackText.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }

        feedbackText.color = new Color(1, 1, 1, 0);
        collectedFeedback.anchoredPosition = originalPosFeedback;
    }

    public void OnItemCollected(string msg)
    {
        StartCoroutine(CollectedFeedbackMsg(msg));
    }
}
