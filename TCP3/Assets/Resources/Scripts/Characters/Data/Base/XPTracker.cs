using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;

public class XPTracker : NetworkBehaviour
{
    public TextMeshProUGUI CurrentLevelText;
    public TextMeshProUGUI CurrentXPText;
    public TextMeshProUGUI XPToNextLevelText;
    public TextMeshProUGUI SkillPoints;
    public VisualEffect VFX_LevelUp;


    [SerializeField] BaseXPTranslation XPTranslationType;

    [SerializeField] UnityEvent<int, int> OnLevelChanged = new UnityEvent<int, int>();
    public UnityEvent onLevelingUpgrade;

    BaseXPTranslation XPTranslation;

    private void Awake()
    {
        var hud = GameObject.Find("PlayerHUD");
        XPTranslation = ScriptableObject.Instantiate(XPTranslationType);
        CurrentLevelText = hud.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        CurrentXPText = hud.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        XPToNextLevelText = hud.transform.GetChild(3).GetChild(2).GetComponent<TextMeshProUGUI>();
        SkillPoints = hud.transform.GetChild(3).GetChild(3).GetComponent<TextMeshProUGUI>();
    }

    public void AddXP(int amount)
    {
        if (!IsOwner) return;

        int previousLevel = XPTranslation.CurrentLevel;
        if (XPTranslation.AddXP(amount))
        {
            OnLevelChanged.Invoke(previousLevel, XPTranslation.CurrentLevel);
            StartCoroutine(LevelUpgrading());
            LevelUpServerRPC();
        }

        RefreshDisplays();
    }

    public void SetLevel(int level)
    {
        int previousLevel = XPTranslation.CurrentLevel;
        XPTranslation.SetLevel(level);

        if (previousLevel != XPTranslation.CurrentLevel)
        {
            OnLevelChanged.Invoke(previousLevel, XPTranslation.CurrentLevel);
        }

        RefreshDisplays();
    }

    void Start()
    {
        RefreshDisplays();

        OnLevelChanged.Invoke(0, XPTranslation.CurrentLevel);
    }

    void RefreshDisplays()
    {
        CurrentLevelText.text = $"Current Level: {XPTranslation.CurrentLevel}";
        CurrentXPText.text = $"Current XP: {XPTranslation.CurrentXP}";
        SkillPoints.text = $"Current SkillPoints: {XPTranslation.SkillPoints}";
        if (!XPTranslation.AtLevelCap)
            XPToNextLevelText.text = $"XP To Next Level: {XPTranslation.XPRequiredForNextLevel}";
        else
            XPToNextLevelText.text = $"XP To Next Level: At Max";
    }

    [ServerRpc]
    void LevelUpServerRPC()
    {
        LevelUpClientRPC();
    }

    [ClientRpc]
    void LevelUpClientRPC()
    {
        if (!IsOwner)
            LevelUpVFX();
    }

    private void LevelUpVFX()
    {
        StartCoroutine(LevelUpgrading());
    }

    public IEnumerator LevelUpgrading()
    {
        onLevelingUpgrade.Invoke();
        yield return new WaitForSeconds(2f);
        VFX_LevelUp.Stop();
    }
}
