using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class UXUIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
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
            // Incrementa os pontos de habilidade
            playerManager.xpTracker.SkillPoints += currentLevel - 1;

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
