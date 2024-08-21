using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventoryController : NetworkBehaviour
{
    private Inventory inventory;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private UI_Inventory uiInventory;

    private void Awake()
    {
        uiInventory = GameManager.Instance.uiInventory;
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        LocatePlayer player = gameObject.GetComponent<LocatePlayer>();
        inventory = new Inventory();
        uiInventory.SetPlayer(player);
        uiInventory.SetInventory(inventory);

        MouseController.CursorVisibility(false);
    }

    public void SetItem(Item item)
    {
        if (inventory.CanPickup())
        {
            this.inventory.AddItem(item);
        }
        else
        {
            //colocar msg popup aq
            Debug.Log("Inventario cheio");
        }
        
    }

    public void RemoveItemByAmount(Item item, int amount)
    {
        this.inventory.RemoveItemByAmount(item, amount);
    }

    public int CountItem(Item item)
    {
        return this.inventory.CountItem(item);
    }

    public bool CanCraft(Craft craft)
    {
        int needConfirms = craft.recipes.Count;
        int currentConfirms = 0;

        foreach (Recipe recipe in craft.recipes)
        {
            if(recipe.needQuantity == CountItem(recipe.item))
                currentConfirms++;
        }

        if (needConfirms == currentConfirms)
        {
            return true;
        }

        Debug.Log("precisava:" + needConfirms.ToString());
        Debug.Log("obteve:" + currentConfirms.ToString());
        return false;
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
