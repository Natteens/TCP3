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

    private void Update()
    {
        VisibilityControl();
    }

    public void SetItem(Item item)
    {
        if (inventory.CanPickup())
        {
            Item newItem = ScriptableObjectUtility.Clone(item);
            newItem.Initialize(item);
            Debug.Log("NewItem amount: " + newItem.amount);
            this.inventory.AddItem(newItem);
        }
        else
        {
            FeedbackManager.Instance.FeedbackText("Inventario cheio!");
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
            // Verifica se a quantidade do item no inventário é suficiente para o craft
            if (CountItem(recipe.item) < recipe.needQuantity)
            {
                Debug.Log("Item insuficiente: " + recipe.item.itemName);
                return false;
            }
        }

        // Se passar por todas as verificações, significa que pode craftar
        return true;
    }

    private void VisibilityControl()
    {
        if (starterAssetsInputs.inventory)
        {
            uiInventory.CheckVisibility();
            starterAssetsInputs.inventory = false;
        }
    }

    public void SelectSlot()
    {
       if (starterAssetsInputs.slot_1) { uiInventory.SelectHotbarSlot(1); starterAssetsInputs.slot_1 = false; }
       if (starterAssetsInputs.slot_2) { uiInventory.SelectHotbarSlot(2); starterAssetsInputs.slot_2 = false; }
       if (starterAssetsInputs.slot_3) { uiInventory.SelectHotbarSlot(3); starterAssetsInputs.slot_3 = false; }
       if (starterAssetsInputs.slot_4) { uiInventory.SelectHotbarSlot(4); starterAssetsInputs.slot_4 = false; }
       if (starterAssetsInputs.slot_5) { uiInventory.SelectHotbarSlot(5); starterAssetsInputs.slot_5 = false; }

    }
}