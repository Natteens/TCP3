using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CraftController : NetworkBehaviour
{
    private CraftInventory inventory;
    [SerializeField] private UI_Craft uiCraft;

    private void Awake()
    {
        if (!IsOwner) return;

        uiCraft = GameManager.Instance.uiCraft;
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
