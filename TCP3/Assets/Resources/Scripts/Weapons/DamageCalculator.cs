using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
    public static int CalculateWithDefense(float defense, float damage)
    {
        float defenseScaleFactor = 120f;

        float damageReduction = defense / (defense + defenseScaleFactor);

        float finalDamage = damage * (1 - damageReduction);

        return Mathf.Max(Mathf.RoundToInt(finalDamage), 1);
    
    }
}
