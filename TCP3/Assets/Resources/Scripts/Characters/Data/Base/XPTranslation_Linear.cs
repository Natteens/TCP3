using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/XP Linear", fileName = "XPTranslation_Linear")]
public class XPTranslation_Linear : BaseXPTranslation
{
    [SerializeField] int Offset = 100;
    [SerializeField] float Slope = 50;
    [SerializeField] int LevelCap = 20;

    protected int XPForLevel(int level)
    {
        return Mathf.FloorToInt(Slope * Mathf.Pow(level - 1, 2)) + Offset;
    }

    public override bool AddXP(int amount)
    {
        if (AtLevelCap)
            return false;

        CurrentXP += amount;

        int newLevel = Mathf.Min(Mathf.FloorToInt(Mathf.Sqrt((CurrentXP - Offset) / Slope)) + 1, LevelCap);
        bool levelledUp = newLevel > CurrentLevel; 

        if (levelledUp)
        {
            CurrentLevel = newLevel;
            AtLevelCap = CurrentLevel == LevelCap;
            CurrentXP = 0;
            SkillPoints++; 
        }

        return levelledUp;
    }


    public override void SetLevel(int level)
    {
        CurrentXP = 0;
        CurrentLevel = 1;
        AtLevelCap = false;

        AddXP(XPForLevel(level));
    }

    protected override int GetXPRequiredForNextLevel()
    {
        if (AtLevelCap)
            return int.MaxValue;

        return XPForLevel(CurrentLevel + 1) - CurrentXP;
    }
}
