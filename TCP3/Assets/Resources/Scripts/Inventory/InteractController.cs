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
        // Verifica se o jogador pressionou o bot�o de intera��o
        if (starterAssetsInputs.interact && currentInteractable != null)
        {
            currentInteractable.OnInteract(transform);
            starterAssetsInputs.interact = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            currentInteractable = interactable; // Armazena o objeto que pode ser interagido
            ControlInteractMessage(true); // Mostra a mensagem de intera��o
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.TryGetComponent<Interactable>(out var interactable) && interactable == currentInteractable)
        {
            currentInteractable = null; // Limpa o objeto atual quando o jogador sai da �rea de intera��o
            ControlInteractMessage(false); // Esconde a mensagem de intera��o
        }
    }
}
