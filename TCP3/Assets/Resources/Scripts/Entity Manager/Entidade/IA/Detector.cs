using UnityEngine;

public abstract class Detector : ScriptableObject
{
    public float targetDetectionRange;

    public abstract void Detect(AIData aiData);

    public abstract void DrawGizmos(AIData aiData);
}
