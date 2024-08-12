using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStatus", menuName = "Status/EntityStatus")]
public class EntityStatus : ScriptableObject
{
    public List<BaseStatus> baseStats;

    private void OnValidate()
    {
        if (baseStats == null || baseStats.Count != System.Enum.GetValues(typeof(StatusType)).Length)
        {
            baseStats.Clear();
            foreach (StatusType statusType in System.Enum.GetValues(typeof(StatusType)))
            {
                baseStats.Add(new BaseStatus { statusType = statusType, value = 0 });
            }
        }
    }
}

[System.Serializable]
public struct BaseStatus
{
    public StatusType statusType;
    public float value;
}
