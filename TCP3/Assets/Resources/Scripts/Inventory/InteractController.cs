using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class InteractController : NetworkBehaviour
{
    [SerializeField] private GameObject interactMessage;
    private StarterAssetsInputs starterAssetsInputs;

    // Tornar visíveis no Inspector
    private Interactable currentInteractable;
    private List<Interactable> interactables = new List<Interactable>();

    private void Awake()
    {
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

        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            interactables.Add(interactable);
            if (currentInteractable == null)
            {
                currentInteractable = interactable;
                ControlInteractMessage(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;

        // Verifica se o currentInteractable ainda é válido
        if (currentInteractable == null || interactables.Contains(currentInteractable) == false)
        {
            // Remove objetos destruídos ou inexistentes da lista
            interactables.RemoveAll(item => item == null);

            // Atualiza o currentInteractable
            if (interactables.Count > 0)
            {
                currentInteractable = interactables[0];
                ControlInteractMessage(true);
            }
            else
            {
                ControlInteractMessage(false);
                return;
            }
        }

        // Verifica se o jogador pressionou o botão de interação
        if (starterAssetsInputs.interact && currentInteractable != null)
        {
            starterAssetsInputs.interact = false;
            currentInteractable.OnInteract(transform);


            // Remove o objeto interagido da lista, caso tenha sido destruído
            interactables.Remove(currentInteractable);

            // Atualiza o currentInteractable
            currentInteractable = interactables.Count > 0 ? interactables[0] : null;
            ControlInteractMessage(currentInteractable != null);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (currentInteractable != null)
        {
            interactables.Remove(currentInteractable);
            currentInteractable = interactables.Count > 0 ? interactables[0] : null;
            ControlInteractMessage(currentInteractable != null);
        }
        
    }

    public void RemoveThisInteractable(Interactable interactable)
    {
        if (interactables.Contains(interactable))
        {
            interactables.Remove(interactable);

            // Atualiza o currentInteractable se o objeto que saiu for o atual
            if (currentInteractable == interactable)
            {
                currentInteractable = interactables.Count > 0 ? interactables[0] : null;
                ControlInteractMessage(currentInteractable != null);
            }
        }
    }
}
