using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftController : MonoBehaviour
{
    private CraftInventory inventory;
    [SerializeField] private UI_Craft uiCraft;

    private void Awake()
    {
        LocatePlayer player = gameObject.GetComponent<LocatePlayer>();
        inventory = new CraftInventory();
        uiCraft.SetPlayer(player);
        uiCraft.SetCraftInventory(inventory);
    }

    public void SetCraft(Craft craft)
    {
        this.inventory.AddCraft(craft);
    }

}
