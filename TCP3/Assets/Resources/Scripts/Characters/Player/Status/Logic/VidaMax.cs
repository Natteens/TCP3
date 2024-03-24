using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats/Constituicao/VidaMax")]
public class VidaMax : Ability
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

