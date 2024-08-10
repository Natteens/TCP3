using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStatus", menuName = "Status/EntityStatus")]
public class EntityStatus : ScriptableObject
{
    public List<BaseStatus> baseStats;
}

[System.Serializable]
public struct BaseStatus
{
    public StatusType statusType;
    public float value;
}
