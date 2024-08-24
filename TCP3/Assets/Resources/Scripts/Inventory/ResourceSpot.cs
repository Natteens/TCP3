using EasyBuildSystem.Packages.Addons.AdvancedBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI; // Importa a biblioteca Odin

public class ResourceSpot : MonoBehaviour, Interactable
{
    [Title("Item Settings")] 
    [SerializeField, Required] 
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)] 
    public Item item;

    [Title("Harvest Settings")]
    [Range(1f, 10f)] 
    public float baseTimeToHarvest;

    [ShowInInspector, ReadOnly] 
    private float maxTime;

    [ShowInInspector, ReadOnly]
    public float currentTime = 0;

    [ShowInInspector, ReadOnly]
    public bool isHarvesting = false;

    private void Start()
    {
        GameManager.Instance.HarvestHolder.SetActive(false);
    }

    void Update()
    {
        if (isHarvesting)
        {
            Countdown();
        }
    }

    public void StartHarvesting()
    {
        isHarvesting = true;
        GameManager.Instance.HarvestHolder.SetActive(true);
        GameManager.Instance.interactMSG.SetActive(false);
    }

    public void CancelHarvesting()
    {
        isHarvesting = false;
        currentTime = 0f;
        GameManager.Instance.HarvestHolder.SetActive(false);
    }

    public void Countdown()
    {
        GameManager.Instance.interactMSG.SetActive(false);
        if (currentTime > maxTime)
        {
            GameManager.Instance.uiInventory.GetPlayer().GetComponent<InventoryController>().SetItem(item);
            currentTime = 0f;
            //isHarvesting = false;
            //GameManager.Instance.HarvestHolder.SetActive(false);
        }
        else
        {
            currentTime += Time.deltaTime;
            GameManager.Instance.HarvestHolder.transform.Find("TimeToHarvest").GetComponent<Image>().fillAmount = currentTime / maxTime;
        }
    }

    public void OnInteract(Transform interactor)
    {
        StartHarvesting();
        maxTime = baseTimeToHarvest - (interactor.GetComponent<StatusComponent>().GetStatus(StatusType.GatheringSpeed) / 10f);
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<InteractController>().RemoveThisInteractable(this);
        CancelHarvesting();
    }
}
