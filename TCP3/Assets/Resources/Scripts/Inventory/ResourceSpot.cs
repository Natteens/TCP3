using EasyBuildSystem.Packages.Addons.AdvancedBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Importa a biblioteca Odin
using UnityEngine.Animations.Rigging;
using Unity.Netcode;

public class ResourceSpot : NetworkBehaviour, Interactable
{
    [Title("Item Settings")] 
    [SerializeField, Required] 
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)] 
    public Item item;

    [Title("Harvest Settings")]
    [Range(1f, 60f)] 
    public float baseTimeToHarvest;
    [Range(1f, 9999f)]
    public int maxResourceToHarvest;

    [ShowInInspector, ReadOnly]
    private float currentResourceToHarvest;

    [ShowInInspector, ReadOnly] 
    private float maxTime;

    [ShowInInspector, ReadOnly]
    private float currentTime = 0;

    [ShowInInspector, ReadOnly]
    private bool isHarvesting = false;

    [ShowInInspector, ReadOnly]
    public NetworkVariable<bool> canHarvest = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [ShowInInspector, ReadOnly]
    private StarterAssetsInputs inputs;

    [ShowInInspector, ReadOnly]
    private InteractController controller;
    
    [ShowInInspector, ReadOnly]
    private Animator anim;

    [ShowInInspector, ReadOnly]
    private MultiAimConstraint torsoAimConstraint;
    private WeightedTransformArray originalSourceObjects;

    private BoxCollider myCollider;


    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        GameManager.Instance.HarvestHolder.SetActive(false);
        currentResourceToHarvest = maxResourceToHarvest;
        currentTime = 0f;
    }

    void Update()
    {
        if (!IsSpawned) return;

        myCollider.enabled = canHarvest.Value;
        //botar aqui o shader ou a cor que vai aplicar quando o recurso nao for possivel de pegar usando o canharvest de parametro
        ResourceSpotLogic();
    }

    private void ResourceSpotLogic()
    {
        if (isHarvesting)
        {
            if (inputs != null)
            {
                if (inputs.move != Vector2.zero)
                {
                    CancelHarvesting();
                    StartCoroutine(FeedbackManager.Instance.FeedbackTextForCancelColect());
                }
            }

            Countdown();
        }
        else if (!canHarvest.Value)
        {
            RestaureResource();
        }
    }

    private void RestaureResource()
    {
        CancelHarvesting();

        if (currentResourceToHarvest < maxResourceToHarvest)
        {
            currentResourceToHarvest += Time.deltaTime;
        }
        else
        {
            currentResourceToHarvest = maxResourceToHarvest;
            canHarvest.Value = true;
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
        //StartCoroutine(SetLayerWeight(anim, 2, 0f, .5f));
        //anim.SetBool("Collection", false);
        GameManager.Instance.HarvestHolder.SetActive(false);
        isHarvesting = false;
        currentTime = 0f;
        inputs = null;
        RestoreOriginalAimSource();
    }

    public void Countdown()
    {
        if (canHarvest.Value)
        {
            if (currentResourceToHarvest <= 0)
            {
                StartCoroutine(FeedbackManager.Instance.FeedbackTextForNoResource());
                GameManager.Instance.HarvestHolder.SetActive(false);
                currentTime = 0f;
                canHarvest.Value = false;
                return;
            }

            if (currentTime > maxTime)
            {
                GameManager.Instance.uiInventory.GetPlayer().GetComponent<InventoryController>().SetItem(item);
                currentResourceToHarvest--;
                currentTime = 0f;
                return;
            }

            currentTime += Time.deltaTime;
            GameManager.Instance.HarvestHolder.transform.Find("TimeToHarvest").GetComponent<Image>().fillAmount = currentTime / maxTime;
            
        }
    }

    public void OnInteract(Transform interactor)
    {
        if (canHarvest.Value)
        {
            inputs = interactor.GetComponent<StarterAssetsInputs>();
            controller = interactor.GetComponent<InteractController>();
            anim = interactor.GetComponentInChildren<Animator>();
            torsoAimConstraint = interactor.GetComponentInChildren<MultiAimConstraint>();
            originalSourceObjects = torsoAimConstraint.data.sourceObjects;

            StartCoroutine(SetLayerWeight(anim, 2, 1f, 1f));
            anim.SetBool("Collection", true);
            UpdateAimSource();

            StartHarvesting();
            maxTime = baseTimeToHarvest - (interactor.GetComponent<StatusComponent>().GetStatus(StatusType.GatheringSpeed) / 15f);
        }
        else
        {
            StartCoroutine(FeedbackManager.Instance.FeedbackTextForRenewingResource());
            #region animator
            StartCoroutine(SetLayerWeight(anim, 2, 0f, .5f));
            anim.SetBool("Collection", false); 
            #endregion
        }
        
    }

    private void UpdateAimSource()
    {
        if (torsoAimConstraint != null)
        {
            var newSourceObjects = torsoAimConstraint.data.sourceObjects;
            newSourceObjects.Clear();
            newSourceObjects.Add(new WeightedTransform(transform, 1f));
            torsoAimConstraint.data.sourceObjects = newSourceObjects;
        }
    }

    private void RestoreOriginalAimSource()
    {
        if (torsoAimConstraint != null && originalSourceObjects.Count > 0)
        {
            torsoAimConstraint.data.sourceObjects = originalSourceObjects;
        }
    }

    private IEnumerator SetLayerWeight(Animator animator, int layerIndex, float targetWeight, float duration)
    {
        float startWeight = animator.GetLayerWeight(layerIndex);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float weight = Mathf.Lerp(startWeight, targetWeight, elapsed / duration);
            animator.SetLayerWeight(layerIndex, weight);
            yield return null;
        }

        animator.SetLayerWeight(layerIndex, targetWeight);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ControlUI();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ControlUI();

            if (inputs != null && inputs.interact)
            {
                if (!isHarvesting && canHarvest.Value)
                {
                    OnInteract(other.transform); 
                }
                else if (isHarvesting)
                {
                    Countdown(); 
                }
            }
        }
    }

    private void ControlUI()
    {

        if (!isHarvesting && canHarvest.Value)
        {
            GameManager.Instance.interactMSG.SetActive(true);
        }
        else if (isHarvesting && canHarvest.Value)
        {
            GameManager.Instance.interactMSG.SetActive(false);
            GameManager.Instance.HarvestHolder.SetActive(true);

        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && IsOwner) 
        {
            controller.RemoveThisInteractable(this);
            controller = null;
            CancelHarvesting();
        }
        
    }
}
