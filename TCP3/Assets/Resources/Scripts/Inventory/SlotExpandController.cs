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
    [SerializeField] private GameObject selectedSlot;
    [SerializeField] private Color baseColor;

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
    public bool hasUse = false;

    public void Start()
    {
        Squeeze();
        slider.onValueChanged.AddListener(UpdateSliderAmount);
        sliderAmount.text = "1";
        selectedSlot = null;
    }


    public void SetSlot(GameObject obj)
    {
        if (selectedSlot != null) Clean();

        if (obj == null) { Debug.Log("SETEI O SLOT COMO NULO"); }
        else { Debug.Log("SETEI O SLOT"); }

        selectedSlot = obj;
    }

    public void Setup(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item passado para Setup é nulo!");
            return;
        }

        selectedItem = item;

        // Atualiza os detalhes do item na interface
        itemName.text = selectedItem.itemName;
        itemDesc.text = selectedItem.itemDescription;
        itemImage.sprite = selectedItem.itemSprite;

        if (item.itemType != Item.Itemtype.Consumivel)
        {
            itemDesc.fontSize = 18;
        }
        else
        {
            itemDesc.fontSize = 15;
        }

        // Define o slider
        sliderMaxAmount = selectedItem.amount;
        slider.maxValue = sliderMaxAmount;
        sliderAmount.text = "1";

        // Verifique se o selectedSlot não é nulo e altere a cor dele
        if (selectedSlot != null)
        {
            Image slotImage = selectedSlot.GetComponentInParent<Image>();  // Pegue a Image diretamente do slot

            if (slotImage != null)
            {
                // Verifica a cor atual e alterna entre vermelho e branco
                if (slotImage.color == Color.white)
                {
                    slotImage.color = new Color(253f / 255f, 109f / 255f, 109f / 255f); // Muda para vermelho claro
                    Debug.Log("Cor do slot alterada para vermelho claro.");
                }
                else if (slotImage.color == new Color(253f / 255f, 109f / 255f, 109f / 255f))
                {
                    slotImage.color = Color.white; // Reseta para branco
                    Debug.Log("Cor do slot resetada para branco.");
                    Squeeze();
                }
            }
            else
            {
                Debug.LogError("selectedSlot não contém um componente Image.");
            }
        }
        else
        {
            Debug.LogWarning("selectedSlot é nulo no Setup.");
        }
    }

    public void Clean()
    {
        // Reseta o slider
        slider.value = 1;
        sliderAmount.text = "1";

        // Se existe um slot selecionado, resetar a cor
        if (selectedSlot != null)
        {
            Image slotImage = selectedSlot.GetComponentInParent<Image>();  // Pegue a Image diretamente do slot
            if (slotImage != null)
            {
                slotImage.color = Color.white; // Resetar para a cor base (branca)
                Debug.Log("Cor do slot resetada para branco.");
            }
            else
            {
                Debug.LogError("selectedSlot não contém um componente Image no Clean.");
            }
        }
        else
        {
            Debug.LogWarning("selectedSlot é nulo no Clean.");
        }

        // Limpar seleção
        selectedSlot = null;
        selectedItem = null;

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

            Spawner.Instance.SpawnItemServerRpc(PlayersManager.Instance.GetMyPlayer().transform.position, itemToDrop.uniqueID);
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
                SurvivalManager manager = PlayersManager.Instance.GetMyPlayer().GetComponent<SurvivalManager>();
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
                    SurvivalManager manager = PlayersManager.Instance.GetMyPlayer().GetComponent<SurvivalManager>();
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

                if (amount > 0)
                {
                    // Crie uma cópia do item antes de alterar o valor
                    Item itemToDrop = ScriptableObjectUtility.Clone(selectedItem);
                    itemToDrop.amount = amount;

                    // Dropa a cópia do item com o valor atualizado
                    Spawner.Instance.SpawnItemServerRpc(PlayersManager.Instance.GetMyPlayer().transform.position + new Vector3(0f, 5f, 0f), itemToDrop.uniqueID);

                    // Remove a quantidade correta do inventário
                    GameManager.Instance.uiInventory.GetInventory().RemoveItemByAmount(selectedItem, amount);

                    Clean();
                }
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
