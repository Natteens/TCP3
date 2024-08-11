using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftInventory
{
    public event EventHandler OncraftListChanged;
    [SerializeField] private List<Craft> craftList;

    public CraftInventory()
    {
        craftList = new List<Craft>();
    }

    public void AddCraft(Craft Craft)
    {
        bool alreadyHave = false;
        foreach (Craft craft in craftList)
        {
            if (craft == Craft) { alreadyHave = true; break; }
        }
        if (!alreadyHave)
        {
            craftList.Add(Craft);
            OncraftListChanged?.Invoke(this, EventArgs.Empty);
        }
        else { Debug.Log("ja possui esse craft!"); }
    }

    public List<Craft> GetcraftList()
    {
        return craftList;
    }
}
