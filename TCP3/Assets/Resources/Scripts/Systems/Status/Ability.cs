using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Ability : SerializedScriptableObject
{
    [SerializeField]
    private float baseValue;

    public float BaseValue
    {
        get { return baseValue; }
        set { baseValue = value; }
    }

    [SerializeField]
    private float upgradePerLevel;

    public float UpgradePerLevel
    {
        get { return upgradePerLevel; }
        set { upgradePerLevel = value; }
    }

    public virtual float GetValue(Status status)
    {
        return BaseValue;
    }

}

