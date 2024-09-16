using CodeMonkey.Utils;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Linq;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;

public class UI_Inventory : NetworkBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform inventoryHolder;
    [SerializeField] private RectTransform[] Slots;
    [SerializeField] private RectTransform[] SlotsHotbar;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private SlotExpandController expandController;
    private LocatePlayer player;
    public bool isVisible = false;
    public List<Item> debugitems;


    private void Start()
    {
        inventoryHolder.gameObject.SetActive(false);
        RefreshInventoryItems();
        CleanHotbarSlots();
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    public void UpdateItemPosition(int oldIndex, int newIndex)
    {
        List<Item> itemList = inventory.GetItemList();

        // Verifique se os índices estão dentro dos limites válidos
        if (oldIndex >= 0 && oldIndex < itemList.Count && newIndex >= 0 && newIndex < Slots.Length)
        {
            Item item = itemList[oldIndex];

            // Remove o item da posição antiga
            itemList[oldIndex] = null;

            // Se o novo índice está dentro dos limites da lista
            if (newIndex < itemList.Count)
            {
                if (itemList[newIndex] != null)
                {
                    SwapItemsInList(itemList, oldIndex, newIndex);   
                }

                itemList[newIndex] = item;
            }
            else
            {
                // Se o novo índice está fora dos limites da lista, adicione o item na última posição
                itemList.Add(item);
            }

            // Atualize a interface do usuário
            RefreshInventoryItems();
        }
    }
    public Inventory GetInventory() { return inventory; }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    public void RefreshInventoryItems()
    {
        List<Item> itemList = inventory.GetItemList();

        // Limpar todos os slots da hotbar antes de configurá-los novamente
        CleanHotbarSlots();

        int hotbarStartIndex = Math.Max(0, itemList.Count - 5);

        for (int i = 0; i < Slots.Length; i++)
        {
            if (i < itemList.Count && itemList[i] != null)
            {
                ConfigureItemSlot(itemList[i], Slots[i]);
            }
            else
            {
                if (Slots[i].childCount > 0)
                {
                    ClearSlot(Slots[i].GetChild(Slots[i].childCount - 1).GetComponent<RectTransform>());
                }
            }

            // Atualizar hotbar para os últimos 5 itens do inventário
            if (i >= hotbarStartIndex && i < itemList.Count)
            {
                int hotbarIndex = i - hotbarStartIndex;
                ConfigureHotbarSlot(itemList[i], SlotsHotbar[hotbarIndex]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && GameManager.Instance.isDebugActive)
        {
            foreach (Item item in debugitems) { inventory.AddItem(item); }
        }
    }

    private void ConfigureItemSlot(Item item, RectTransform rect)
    {
        
        if (rect.childCount > 0)
        {
            ClearSlot(rect.GetChild(rect.childCount - 1).GetComponent<RectTransform>());
        }

        GameObject instance = Instantiate(itemSlotPrefab, rect);

        instance.GetComponent<Button_UI>().ClickFunc = () =>
        {
            Debug.Log("Meu item é:" + item.itemName);
            expandController.SetSlot(instance);
            expandController.Setup(item);
            expandController.hasUse = false;

            if (item != null && item.itemType == Item.Itemtype.Consumivel)
            {
                expandController.hasUse = true;
            }

            expandController.ActiveItemButton();
            expandController.Expand();
        };

        // Configurar o item arrastável
        DraggableItem draggableItem = instance.GetComponent<DraggableItem>();
        draggableItem.SetItem(item);

        // Configurar a tooltip do item
        instance.GetComponent<ItemTooltip>().SetItem(item);

        Image img = instance.GetComponent<Image>();
        TextMeshProUGUI txt = instance.GetComponentInChildren<TextMeshProUGUI>();

        if (img != null && txt != null)
        {
            if (item != null)
            {
                img.sprite = item.itemSprite;
                txt.text = item.amount.ToString();
            }
            else
            {
                img.sprite = null;
                txt.text = "";
            }
        }
    } 

    private void ClearSlot(RectTransform obj)
    {
        Destroy(obj.gameObject);
    }

    public LocatePlayer GetPlayer()
    {
        return player;
    }
    public void SetPlayer(LocatePlayer player)
    {
        this.player = player;
    }

    public void CheckVisibility()
    {
        isVisible = !isVisible;

        if (isVisible)
        {
            //Debug.Log("#Ativei o inventario#");
            GameManager.Instance.uiCraft.ControlExpandedCraft(false);
            //expandController.Squeeze();
            expandController.Clean();
            MouseController.CursorVisibility(true);
        }
        else
        {
            //Debug.Log("#Desativei o inventario#");
            MouseController.CursorVisibility(false);
            TooltipScreenSpaceUI.HideTooltip_Static();
        }

        inventoryHolder.gameObject.SetActive(isVisible);
    }

    public void SwapItemsInList<T>(List<T> list, int indexA, int indexB)
    {
        if (indexA != indexB && indexA >= 0 && indexB >= 0 && indexA < list.Count && indexB < list.Count)
        {
            T temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }
    }

    private void ConfigureHotbarSlot(Item item, RectTransform rect)
    {
        if (item && rect != null)
        {
            Image image = rect.gameObject.transform.Find("image").GetComponent<Image>();
            image.enabled = true;
            TextMeshProUGUI amount = rect.gameObject.transform.Find("txt").GetComponent<TextMeshProUGUI>();
            amount.enabled = true;

            image.sprite = item.itemSprite;
            amount.text = "x" + item.amount.ToString();
        }
    }

    public void SelectHotbarSlot(int id)
    {
        if (id <= 0) return;

        ChangeHotbarBgColor(id-1);
        ConfigureHotbar();
    }

    private void ChangeHotbarBgColor(int id)
    {
        ClearHotbarBg(id);
        Image currentSlotBg = SlotsHotbar[id].Find("bg").GetComponent<Image>();

        if (currentSlotBg.color == Color.red)
        {
            currentSlotBg.color = Color.white;
            return;
        }

        currentSlotBg.color = Color.red;
        
    }

    private void ConfigureHotbar()
    {
        // Verificar o slot selecionado e pegar o item
        for (int i = 0; i < SlotsHotbar.Length; i++)
        {
            // Verifica se o background do slot está vermelho (indica que foi selecionado)
            Image bgImage = SlotsHotbar[i].Find("bg").GetComponent<Image>();
            if (bgImage.color == Color.red)
            {
                // Pegue o item do slot
                List<Item> itemList = inventory.GetItemList();
                int hotbarStartIndex = Math.Max(0, itemList.Count - 5);
                int itemIndex = hotbarStartIndex + i;

                if (itemIndex < itemList.Count && itemList[itemIndex] != null)
                {
                    Item item = itemList[itemIndex];
                    UseItem(item); // Usa o item
                }
            }
        }
    }

    // Função para utilizar o item da hotbar
    private void UseItem(Item item)
    {
        if (item == null) return;

        DeactiveWeapon();

        if (item.itemType == Item.Itemtype.Consumivel)
        {
            Consume(item);
        }
        else if (item.itemType == Item.Itemtype.Arma)
        {
            Debug.Log($"Equipando arma: {item.itemName}");
            ActiveWeapon(item);
        }
        else
        {
            Debug.Log($"Item {item.itemName} não é utilizável.");
            DeactiveWeapon();
        }
    }

    private void Consume(Item item)
    {
        Debug.Log($"Usando item consumível: {item.itemName}");
        Consumable consumable = item as Consumable;

        if (consumable != null)
        {
            SurvivalManager manager = GameManager.Instance.uiInventory.GetPlayer().gameObject.GetComponent<SurvivalManager>();
            manager.IncreaseStats(consumable);
        }

        item.amount--;
        if (item.amount <= 0)
        {
            inventory.RemoveItem(item); // Remover o item do inventário se quantidade for 0
        }
        RefreshInventoryItems(); // Atualiza a interface do inventário
    }

    private void ActiveWeapon(Item item)
    {
        if(!IsOwner) return;
        WeaponController weaponController = GameManager.Instance.uiInventory.GetPlayer().gameObject.GetComponent<WeaponController>();
        WeaponInfo cWeapon = item as WeaponInfo;

        weaponController.currentWeapon = cWeapon;
        weaponController.StartingWeaponServerRpc();
    }

    private void DeactiveWeapon()
    {
        WeaponController weaponController = GameManager.Instance.uiInventory.GetPlayer().gameObject.GetComponent<WeaponController>();

        weaponController.DeactivateCurrentWeaponServerRpc();
    }

    public void ClearHotbarBg(int idForExclude)
    {
        for (int i = 0; i < SlotsHotbar.Length; i++)
        {
            if (i != idForExclude) 
            SlotsHotbar[i].Find("bg").GetComponent<Image>().color = Color.white; 
        }
    }

    private void CleanHotbarSlots()
    {
        foreach (RectTransform hotbarSlot in SlotsHotbar)
        {
            Image hotbarImage = hotbarSlot.Find("image").GetComponent<Image>();
            hotbarImage.sprite = null; // Adicionado para remover qualquer referência à imagem anterior
            hotbarImage.enabled = false;

            TextMeshProUGUI hotbarText = hotbarSlot.Find("txt").GetComponent<TextMeshProUGUI>();
            hotbarText.text = ""; // Adicionado para limpar o texto
            hotbarText.enabled = false;
        }
    }

}
