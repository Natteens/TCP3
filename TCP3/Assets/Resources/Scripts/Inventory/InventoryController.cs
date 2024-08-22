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
        inventory = new Inventory();
        LocatePlayer player = gameObject.GetComponent<LocatePlayer>();
        uiInventory.SetPlayer(player);
        uiInventory.SetInventory(inventory);

        MouseController.CursorVisibility(false);
    }

    public void SetItem(Item item)
    {
        if (inventory.CanPickup())
        {
            this.inventory.AddItem(item);
            Debug.Log("Adicionando item!");
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
        foreach (Recipe recipe in craft.recipes)
        {
            // Verifica se a quantidade do item no invent�rio � suficiente para o craft
            if (CountItem(recipe.item) < recipe.needQuantity)
            {
                Debug.Log("Item insuficiente: " + recipe.item.itemName);
                return false;
            }
        }

        // Se passar por todas as verifica��es, significa que pode craftar
        return true;
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