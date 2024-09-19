using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CreateCraft : MonoBehaviour
{
    public Craft currentCraft;
    [SerializeField] private UI_Inventory uiInventory;

    public void Start()
    {
        uiInventory = GameManager.Instance.uiInventory;
    }

    public void CreateItem()
    {

        InventoryController inventory = PlayersManager.Instance.GetMyPlayer().GetComponent<InventoryController>();

        if (currentCraft == null)
        {
            Debug.LogError("Craft não setado!");
            return;
        }

        if (inventory.CanCraft(currentCraft))
        {
            // Garante que o item a ser adicionado seja uma nova instância, evitando problemas de referência
            Item craftedItem = ScriptableObject.Instantiate(currentCraft.outputItem);
            inventory.SetItem(craftedItem);

            GameManager.Instance.uiInventory.RefreshInventoryItems();

            // Remove os itens necessários do inventário
            foreach (Recipe recipe in currentCraft.recipes)
            {
                inventory.RemoveItemByAmount(recipe.item, recipe.needQuantity);
            }

            // Caso você tenha uma função que atualize o texto expandido, chame ela aqui
            UI_Craft craftUI = GameManager.Instance.uiCraft;
            if (craftUI != null)
            {
                craftUI.ConfigureExpandedCraft(currentCraft); // Atualiza o texto expandido
            }

            return;
        }

        Debug.LogError("Não estou permitido a craftar!");
    }
}
