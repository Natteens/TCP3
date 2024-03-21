using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AbilityLevel
{
    //CON
    public float RegenerationHP;
    public float RegenerationVigor;
    public float ReductionFome;
    public float ReductionDamage;
    // STR
    public float MeleeDamage;
    // AGL
    public float ReductionRecargaRanged;
    public float ReductionCustoVigor;
    public float MoveSpeed;
    public float SprintSpeed;
    public float JumpHeight;
    // PRC
    public float RangedDamage;
    // LCK
    public float Loot;
}

[CreateAssetMenu(menuName = "Player Stats")]
public class Stats : ScriptableObject
{
    public List<AbilityLevel> constitutionLevels;
    public List<AbilityLevel> strengthLevels;
    public List<AbilityLevel> agilityLevels;
    public List<AbilityLevel> precisionLevels;
    public List<AbilityLevel> luckLevels;
}
