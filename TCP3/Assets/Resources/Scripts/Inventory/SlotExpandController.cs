using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

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
    [FoldoutGroup("UI Containers")]
    [SerializeField] private GameObject selectSlot;
    [SerializeField] private Color saveColorSlot;

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
        sliderAmount.text = "1";
        selectSlot = null;
    }


    public void SetSlot(GameObject obj)
    {
        selectSlot = obj;
    }

    public void Setup(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item passado para Setup é nulo!");
            return;
        }
        Debug.Log(item.amount);
        selectedItem = item;
        Debug.Log(selectedItem.amount);

        itemName.text = selectedItem.itemName;
        itemDesc.text = selectedItem.itemDescription;
        itemImage.sprite = selectedItem.itemSprite;

        sliderMaxAmount = selectedItem.amount;
        slider.maxValue = sliderMaxAmount;
        sliderAmount.text = "1";

        if (selectSlot != null) saveColorSlot = selectSlot.GetComponentInChildren<Image>().color;
        if (selectSlot != null) selectSlot.GetComponentInChildren<Image>().color = Color.red;
    }

    public void Clean()
    {
        selectedItem = null;
        slider.value = 1;
        sliderAmount.text = "1";
        if(selectSlot != null) selectSlot.GetComponentInChildren<Image>().color = saveColorSlot;
        selectSlot = null;
        Squeeze();
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
            int amountToDrop = 1;
            Item itemToDrop = ScriptableObjectUtility.Clone(selectedItem);
            itemToDrop.amount = amountToDrop;

            Spawner.Instance.SpawnItemServerRpc(GameManager.Instance.uiInventory.GetPlayer().GetPosition(), itemToDrop.uniqueID);
            GameManager.Instance.uiInventory.GetInventory().RemoveItemByAmount(selectedItem, amountToDrop);

            Clean();
            Squeeze();
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
                Squeeze();
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
        Debug.Log("SelecAmount:" + selectedItem.itemName);

        if (selectedItem == null)
        {
            Debug.LogError("SELECTED ITEM NULO no início de SelectAmountConfirm!");
            ActiveInfo();
            return;
        }

        if (isConsumable)
        {
            if (selectedItem != null)
            {
                Consumable consumable = selectedItem as Consumable;
                if (consumable != null)
                {
                    SurvivalManager manager = GameManager.Instance.uiInventory.GetPlayer().gameObject.GetComponent<SurvivalManager>();
                    int amount = int.Parse(sliderAmount.text);
                    for (int i = 0; i < amount; i++)
                    {
                        manager.IncreaseStats(consumable); 
                    }

                    Debug.Log("Debug em SelectAmount:" + selectedItem.itemName);
                    GameManager.Instance.uiInventory.GetInventory().RemoveItemByAmount(selectedItem, amount);
                    Clean();
                    return;
                }   
            }
            Debug.Log("SELECTED ITEM NULO CONSUMIVEL");
        }
        else //Se nao e consumivel entao é pra dropar
        {
            if (selectedItem != null)
            {
                int amount = int.Parse(sliderAmount.text);

                // Crie uma cópia do item antes de alterar o valor
                Item itemToDrop = ScriptableObjectUtility.Clone(selectedItem);
                itemToDrop.amount = amount;

                // Dropa a cópia do item com o valor atualizado
                Spawner.Instance.SpawnItemServerRpc(GameManager.Instance.uiInventory.GetPlayer().GetPosition(), itemToDrop.uniqueID);

                // Remove a quantidade correta do inventário
                GameManager.Instance.uiInventory.GetInventory().RemoveItemByAmount(selectedItem, amount);

                Clean();
                return;
            }

            Debug.Log("SELECTED ITEM NULO DROPAVEL");
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
