using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CreateCraft : MonoBehaviour
{
    public Craft currentCraft;
    [SerializeField] private UI_Inventory uiInventory;
    [SerializeField] private LocatePlayer player;

    public void Start()
    {
        uiInventory = GameManager.Instance.uiInventory;
        player = uiInventory.GetPlayer();
    }

    public void CreateItem()
    {
        InventoryController inventory = player.GetComponent<InventoryController>();

        if (inventory.CanCraft(currentCraft))
        {
            inventory.SetItem(currentCraft.outputItem);

            foreach (Recipe recipe in currentCraft.recipes)
            {
                inventory.RemoveItemByAmount(recipe.item, recipe.needQuantity);
            }
        }
    }
}
