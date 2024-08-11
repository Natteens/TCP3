using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private Inventory inventory;
    private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private UI_Inventory uiInventory;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        LocatePlayer player = gameObject.GetComponent<LocatePlayer>();
        inventory = new Inventory();
        uiInventory.SetPlayer(player);
        uiInventory.SetInventory(inventory);
    }

    public void SetItem(Item item)
    {
        this.inventory.AddItem(item);
    }

    private void Update()
    {
        VisibilityControl();
    }

    private void VisibilityControl()
    {
        if (starterAssetsInputs.inventory)
        {
            uiInventory.CheckVisibility();
            starterAssetsInputs.inventory = false;
        }
    }

}
