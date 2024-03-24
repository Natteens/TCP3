using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats/Agilidade/MoveSpeed")]
public class MoveSpeed : Ability
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