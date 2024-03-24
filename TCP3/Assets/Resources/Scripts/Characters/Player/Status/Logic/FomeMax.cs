using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats/Constituicao/FomeMax")]
public class FomeMax : Ability
{
    public override float GetValue(Status status)
    {
        if (status.constitution > 1)
        {
            return BaseValue + (UpgradePerLevel * status.constitution);
        }
        else
        {
            return BaseValue;
        }
    }
}
