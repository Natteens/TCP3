using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CraftController : NetworkBehaviour
{
    private CraftInventory inventory;
    [SerializeField] private UI_Craft uiCraft;

    //DEBUG
    public List<Craft> debugcrafts;

    private void Awake()
    {
        uiCraft = GameManager.Instance.uiCraft;
        LocatePlayer player = gameObject.GetComponent<LocatePlayer>();
        inventory = new CraftInventory();
        uiCraft.SetPlayer(player);
        uiCraft.SetCraftInventory(inventory);
    }

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.K))
        { 
            foreach (Craft craft in debugcrafts) { SetCraft(craft); }
        }
    }

    public void SetCraft(Craft craft)
    {
        this.inventory.AddCraft(craft);
    }

}
