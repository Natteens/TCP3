using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SlotExpandController : MonoBehaviour
{
    [FoldoutGroup("Item Settings")]
    [SerializeField] private Item selectedItem;

    [FoldoutGroup("UI Containers")]
    [SerializeField] private GameObject slotExpandContainer;
    [FoldoutGroup("UI Containers")]
    [SerializeField] private GameObject infoContainer;
    [FoldoutGroup("UI Containers")]
    [SerializeField] private GameObject selectAmountContainer;

    [FoldoutGroup("Buttons")]
    [SerializeField] private GameObject dropButton;
    [FoldoutGroup("Buttons")]
    [SerializeField] private GameObject useButton;

    [FoldoutGroup("Slider Settings")]
    [SerializeField] private Slider slider;
    [FoldoutGroup("Slider Settings")]
    [SerializeField] private TextMeshProUGUI sliderAmount;
    [FoldoutGroup("Slider Settings")]
    [SerializeField] private int sliderMaxAmount;

    [FoldoutGroup("Item Display")]
    [SerializeField] private TextMeshProUGUI itemName;
    [FoldoutGroup("Item Display")]
    [SerializeField] private TextMeshProUGUI itemDesc;
    [FoldoutGroup("Item Display")]
    [SerializeField] private Image itemImage;

    [FoldoutGroup("Miscellaneous")]
    public bool hasUse;

    public void Start()
    {
        Squeeze();
        slider.onValueChanged.AddListener(UpdateSliderAmount);
        sliderAmount.text = "0";
    }

    public void Setup(Item item)
    { 
        selectedItem = item;
        itemName.text = item.itemName;
        itemDesc.text = item.itemDescription;
        itemImage.sprite = item.itemSprite;
        sliderMaxAmount = item.amount;
        slider.maxValue = sliderMaxAmount;
        sliderAmount.text = "0";
    }

    public void Clean()
    {
        selectedItem = null;
        slider.value = 0;
        sliderAmount.text = "0";
    }

    public void ActiveItemButton()
    { 
        dropButton.SetActive(true);
        useButton.SetActive(hasUse);
    }

    public void DeactiveItemButton()
    {
        dropButton.SetActive(false);
        useButton.SetActive(false);
    }

    private void ActiveInfo()
    { 
        infoContainer.SetActive(true);
        selectAmountContainer.SetActive(false);
    }

    private void ActiveSelectAmount()
    {
        infoContainer.SetActive(false);
        selectAmountContainer.SetActive(true);
    }

    public void DropButton()
    {
        if (selectedItem.amount > 1)
        {
            ActiveSelectAmount();
            return;
        }

        if (selectedItem != null)
        {
            GameManager.Instance.uiInventory.GetInventory().RemoveItem(selectedItem);
            ItemWorld.DropItem(GameManager.Instance.uiInventory.GetPlayer().GetPosition(), selectedItem);
            Clean();
        }
    }

    public void UseButton()
    {
        if (selectedItem.amount > 1)
        {
            ActiveSelectAmount();
            return;
        }

        if (selectedItem != null)
        {
            Consumable consumable = selectedItem as Consumable;
            if (consumable != null)
            {
                SurvivalManager manager = GameManager.Instance.uiInventory.GetPlayer().gameObject.GetComponent<SurvivalManager>();
                manager.IncreaseStats(consumable);
                GameManager.Instance.uiInventory.GetInventory().RemoveItem(selectedItem);
                Clean();
            }
        }
    }

    public void Expand()
    { 
        slotExpandContainer.SetActive(true);
    }

    public void Squeeze()
    {
        slotExpandContainer.SetActive(false);
    }

    public void SelectAmountConfirm()
    {
        bool isConsumable = selectedItem.itemType == Item.Itemtype.Consumivel;

        if (isConsumable)
        {
            if (selectedItem != null)
            {
                Consumable consumable = selectedItem as Consumable;
                if (consumable != null)
                {
                    SurvivalManager manager = GameManager.Instance.uiInventory.GetPlayer().gameObject.GetComponent<SurvivalManager>();
                    manager.IncreaseStats(consumable);
                    int amount = int.Parse(sliderAmount.text);
                    GameManager.Instance.uiInventory.GetInventory().RemoveItemByAmount(selectedItem, amount);
                    Clean();
                }   
            }
        }
        else //Se nao e consumivel entao é pra dropar
        {
            if (selectedItem != null)
            {
                int amount = int.Parse(sliderAmount.text);

                GameManager.Instance.uiInventory.GetInventory().RemoveItemByAmount(selectedItem,amount);
                ItemWorld.DropItem(GameManager.Instance.uiInventory.GetPlayer().GetPosition(), selectedItem);
                Clean();
            }
        }
        ActiveInfo();
    }

    public void SelectAmountCancel()
    {
        ActiveInfo();
    }

    private void UpdateSliderAmount(float value)
    {
        sliderAmount.text = value.ToString("0"); // Atualiza o texto com o valor do slider
    }

}
