using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;
using QFSW.QC;

public class XPTracker : NetworkBehaviour
{
    [SerializeField]private TextMeshProUGUI CurrentLevelText;
    [SerializeField]private TextMeshProUGUI CurrentXPText;
    [SerializeField]private TextMeshProUGUI XPToNextLevelText;
    [SerializeField]private TextMeshProUGUI SkillPointsText;
    [SerializeField]private BaseXPTranslation XPTranslationType;
    [SerializeField]private GameObject VFX_LevelUp;
    [SerializeField]private Transform spawnPoint;

    UnityEvent<int, int> OnLevelChanged = new UnityEvent<int, int>();
    BaseXPTranslation XPTranslation;
    public int SkillPoints
    {
        get { return XPTranslation.SkillPoints; }
        set { XPTranslation.SkillPoints = value; }
    }


    private void Awake()
    {
        XPTranslation = ScriptableObject.Instantiate(XPTranslationType);
    }

    [Command]
    public void AddXP(int amount)
    {
        if (!IsOwner) return;

        int previousLevel = XPTranslation.CurrentLevel;
        if (XPTranslation.AddXP(amount))
        {
            OnLevelChanged.Invoke(previousLevel, XPTranslation.CurrentLevel);
            VFXManager.Instance.PlayVFX(VFX_LevelUp, spawnPoint.position, spawnPoint.rotation, 2f);
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

    private void Update()
    {
        RefreshDisplays();
    }

    void RefreshDisplays()
    {
        CurrentLevelText.text = $"Current Level: {XPTranslation.CurrentLevel}";
        CurrentXPText.text = $"Current XP: {XPTranslation.CurrentXP}";
        SkillPointsText.text = $"Current SkillPoints: {XPTranslation.SkillPoints}";
        if (!XPTranslation.AtLevelCap)
            XPToNextLevelText.text = $"XP To Next Level: {XPTranslation.XPRequiredForNextLevel}";
        else
            XPToNextLevelText.text = $"XP To Next Level: At Max";
    }
}
