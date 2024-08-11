using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Craft")]
public class Craft : ScriptableObject
{
    public List<Recipe> recipes;
    public Item outputItem;
}
