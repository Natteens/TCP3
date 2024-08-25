using UnityEngine;

public static class ScriptableObjectUtility
{
    public static T Clone<T>(T original) where T : ScriptableObject
    {
        return Object.Instantiate(original);
    }
}
