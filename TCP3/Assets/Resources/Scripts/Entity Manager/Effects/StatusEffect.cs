using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Effects/New Effect")]
public class StatusEffect : ScriptableObject
{
    [BoxGroup("Setup")] public string effectName;
    [BoxGroup("Setup")] public StatusType type;
    [BoxGroup("Setup")] public float duration;
    [BoxGroup("Setup")] public float interval;
    [BoxGroup("Setup")] public float effectValue;
    [BoxGroup("Setup")] public bool isBuff;
    [BoxGroup("Setup")] public bool isContinuous;
    [BoxGroup("Setup")] public bool isPermanent;
    [BoxGroup("Setup")] public GameObject effectPrefab;

    public StatusEffectData CreateEffectData()
    {
        return new StatusEffectData(
            effectName,
            type,
            duration,
            interval,
            effectPrefab,
            effectValue,
            isBuff,
            isContinuous,
            isPermanent
        );
    }
}
