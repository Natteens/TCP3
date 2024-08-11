using CodeMonkey.Utils;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private CraftInventory craftInventory;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private Transform craftExpandedContainer;
    [SerializeField] private Transform craftExpandedRequirementContainer;
    [SerializeField] private Transform craftRequirementTemplate;
    private LocatePlayer player;
    private Item.Itemtype actualFilter;
    private bool hasExpanded = false;
  

    private void Awake()
    {
        itemSlotContainer = GameObject.Find("CraftSlotContainer").transform;
        itemSlotTemplate = itemSlotContainer.transform.Find("CraftRecipeTemplate");

        craftExpandedContainer = GameObject.Find("CraftExpandContainer").transform;
        craftExpandedRequirementContainer = craftExpandedContainer.Find("RequirementsContainer");
        craftRequirementTemplate = craftExpandedRequirementContainer.Find("RequirementsTemplate");
    }

    private void Start()
    {
        actualFilter = Item.Itemtype.None;
        ControlExpandedCraft(false);
        gameObject.SetActive(false);
        craftExpandedContainer.gameObject.SetActive(false);
    }

    public void SetCraftInventory(CraftInventory CraftInventory)
    {
        this.craftInventory = CraftInventory;
        CraftInventory.OncraftListChanged += CraftInventory_OnItemListChanged;
        RefreshCraftInventoryItems();
    }

    public void SetPlayer(LocatePlayer player)
    {
        this.player = player;
    }

    private void CraftInventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshCraftInventoryItems();
        //Adicionar popUp da receita adicionada
    }
    private void RefreshCraftInventoryItems()
    {
        if (actualFilter == Item.Itemtype.None)
        {
            foreach (Transform child in itemSlotContainer)
            {
                if (child == itemSlotTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (Craft craft in craftInventory.GetcraftList())
            {
                RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                ConfigureCraftSlot(craft, itemSlotRectTransform);
            }
        }
        else 
        {
            FilterRefresh(actualFilter);
        }
     
    }

    public void FilterConsumable()
    {
        ClickOnFilter(Item.Itemtype.Consumable);
    }

    public void FilterWeapon()
    {
        ClickOnFilter(Item.Itemtype.Weapon);
    }

    public void FilterResource()
    {
        ClickOnFilter(Item.Itemtype.Resource);
    }
    public void FilterModule()
    {
        ClickOnFilter(Item.Itemtype.Module);
    }

    public void ControlExpandedCraft(bool control)
    {
        hasExpanded = control;
        craftExpandedContainer.gameObject.SetActive(control);
    }

    private void ClickOnFilter(Item.Itemtype newFilter)
    {
        ControlExpandedCraft(false);
        if (actualFilter == Item.Itemtype.None || newFilter != actualFilter)
        {
            actualFilter = newFilter;
            FilterRefresh(actualFilter);
        }
        else
        {
            actualFilter = Item.Itemtype.None;
            RefreshCraftInventoryItems();
        }
    }

    public void FilterRefresh(Item.Itemtype itemtype)
    {
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Craft craft in craftInventory.GetcraftList())
        {
            if (craft.outputItem.itemType == itemtype)
            {
                RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                ConfigureCraftSlot(craft, itemSlotRectTransform);
            }
        }
    }

    private void ConfigureCraftSlot(Craft craft, RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.gameObject.GetComponent<Button_UI>().ClickFunc = () =>
        {
            if (hasExpanded)
            {
                ConfigureExpandedCraft(craft);
            }
            else
            {
                ControlExpandedCraft(true);
                ConfigureExpandedCraft(craft);
            }
        };

        Image img = rect.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = rect.Find("name").GetComponent<TextMeshProUGUI>();

        if (txt == null) { Debug.LogError("txt nao encontrado"); }
        if (img == null) { Debug.LogError("img nao encontrado"); }

        img.sprite = craft.outputItem.itemSprite;
        txt.text = craft.outputItem.itemName;
        Debug.Log("receita nova: " + craft.outputItem.itemName);
    }

    private void ConfigureExpandedCraft(Craft craft)
    { 
        Image img = craftExpandedContainer.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = craftExpandedContainer.Find("infotxt").GetComponent<TextMeshProUGUI>();
        bool isSameCraft = craft.outputItem.itemDescription == txt.text && craft.outputItem.itemSprite == img.sprite;

        if (isSameCraft && !hasExpanded) 
        {
            ControlExpandedCraft(false);
            return;
        }

        img.sprite = craft.outputItem.itemSprite;
        txt.text = craft.outputItem.itemDescription; 

        foreach (Transform child in craftExpandedRequirementContainer)
        {
            if(child != null && child != craftRequirementTemplate)
            Destroy(child.gameObject);
        }

        foreach (Recipe recipe in craft.recipes)
        {
            if (craftRequirementTemplate == null) craftRequirementTemplate = craftExpandedRequirementContainer.Find("RequirementsTemplate");

            if (craftRequirementTemplate != null)
            {
                RectTransform r = Instantiate(craftRequirementTemplate, craftExpandedRequirementContainer).GetComponent<RectTransform>();
                if (r != null)
                { 
                    r.gameObject.SetActive(true);
                    ConfigureRecipe(recipe, r);
                } 
            }
            else
            {
                Debug.Log("template esta nulo");
            }
        }
    }

    private void ConfigureRecipe(Recipe recipe, RectTransform rect)
    {
        Image img = rect.Find("image").GetComponent<Image>();
        TextMeshProUGUI txt = rect.Find("text").GetComponent<TextMeshProUGUI>();

        if (img == null) Debug.Log("deu bosta");

        img.sprite = recipe.item.itemSprite;
        txt.text = "0" + "/" + recipe.needQuantity; //mudar pra quantia do jogador
    }
}
