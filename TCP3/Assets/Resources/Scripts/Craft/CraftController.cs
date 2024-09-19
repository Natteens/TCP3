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
        uiCraft = GameManager.Instance.uiCraft;
        SetupPlayer player = gameObject.GetComponent<SetupPlayer>();
        inventory = new CraftInventory();
        uiCraft.SetPlayer(player);
        uiCraft.SetCraftInventory(inventory);
    }

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.K) && GameManager.Instance.isDebugActive)
        { 
            foreach (Craft craft in uiCraft.Level1_4) { SetCraft(craft); }
            foreach (Craft craft in uiCraft.Level5_9) { SetCraft(craft); }
            foreach (Craft craft in uiCraft.Level10_15) { SetCraft(craft); }
        }
    }

    public void SetCraft(Craft craft)
    {
        this.inventory.AddCraft(craft);
    }

}
