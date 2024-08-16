using Mono.CSharp;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InteractController : NetworkBehaviour
{
    [SerializeField] private GameObject interactMessage;
    private StarterAssetsInputs starterAssetsInputs;


    private void Awake()
    {
        //ignora colisao entre layer 6 (Player) e 11 (ItemDrop)
        Physics.IgnoreLayerCollision(6, 11);
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        interactMessage = GameManager.Instance.interactMSG;
    }

    public void ControlInteractMessage(bool request)
    { 
        interactMessage.SetActive(request);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if(other.TryGetComponent<Interactable>(out var i))
        ControlInteractMessage(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;
        Interactable i = other.GetComponent<Interactable>(); 
        if (starterAssetsInputs.interact && i != null)
        {
            i.OnInteract(transform);
            starterAssetsInputs.interact = false;
        }
        else if (starterAssetsInputs.interact && i == null){ Debug.LogError("Nao foi encontrado a interface Interactable"); }
    }

    private void OnTriggerExit(Collider other)
    {
        ControlInteractMessage(false);
    }
}
