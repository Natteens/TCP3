using UnityEngine;

public static class ScriptableUtility
{
    public static T Clone<T>(T original) where T : ScriptableObject
    {
        return Object.Instantiate(original);
    }
}
