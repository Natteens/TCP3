using UnityEngine;
using Unity.Netcode;

public class InteractController : NetworkBehaviour
{
    [SerializeField] private GameObject interactMessage;
    private StarterAssetsInputs starterAssetsInputs;
    private Interactable currentInteractable; // Armazena o objeto atual que pode ser interagido

    private void Awake()
    {
        Physics.IgnoreLayerCollision(6, 11); // Ignora colis�o entre layer 6 (Player) e 11 (ItemDrop)
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        interactMessage = GameManager.Instance.interactMSG;
    }

    public void ControlInteractMessage(bool request)
    {
        interactMessage.SetActive(request);
    }

    private void Update()
    {
        if (starterAssetsInputs.interact && currentInteractable != null)
        {
            // Processa a intera��o e desativa a intera��o com o item imediatamente
            currentInteractable.OnInteract(transform);
            currentInteractable = null; // Limpa o objeto atual
            starterAssetsInputs.interact = false;
            ControlInteractMessage(false); // Esconde a mensagem de intera��o
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            // Armazena o objeto que pode ser interagido
            currentInteractable = interactable;
            ControlInteractMessage(true); // Mostra a mensagem de intera��o
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.TryGetComponent<Interactable>(out var interactable) && interactable == currentInteractable)
        {
            // Limpa o objeto atual quando o jogador sai da �rea de intera��o
            currentInteractable = null;
            ControlInteractMessage(false); // Esconde a mensagem de intera��o
        }
    }
}
