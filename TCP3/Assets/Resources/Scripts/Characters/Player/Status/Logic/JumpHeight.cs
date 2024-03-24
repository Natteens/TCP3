using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats/Agilidade/JumpHeight")]
public class JumpHeight : Ability
{
    public override float GetValue(Status status)
    {
        if (status.agility > 1)
        {
            return BaseValue + (UpgradePerLevel * status.agility);
        }
        else
        {
            return BaseValue;
        }
    }
}