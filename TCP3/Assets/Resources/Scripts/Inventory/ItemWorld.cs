using UnityEngine;
using Unity.Netcode;
using CodeMonkey.Utils;
using EasyBuildSystem.Packages.Addons.AdvancedBuilding;
using System.Collections;

public class ItemWorld : NetworkBehaviour, Interactable
{
    [SerializeField] private Item item;

    private void Start()
    {
        StartCoroutine(Lifetime());
    }
    public void SetItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null in SetItem!");
            return;
        }

        Debug.Log(item);
        this.item = item;
    }

    public Item GetItem()
    {
        return item;
    }

    public IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(30f);
        DestroySelf();
    }

    public void OnInteract(Transform interactor)
    {
        if (IsServer)
        {
            // Se já estiver no servidor, processa a interação diretamente
            ProcessInteraction(interactor);
        }
        else
        {
            // Caso contrário, chama o ServerRpc para processar a interação no servidor
            OnInteractClientRpc(NetworkObject.NetworkObjectId, interactor.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ClientRpc(RequireOwnership = false)]
    private void OnInteractClientRpc(ulong itemNetworkObjectId, ulong interactorNetworkObjectId)
    {
        NetworkObject itemNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[itemNetworkObjectId];
        NetworkObject interactorNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[interactorNetworkObjectId];

        if (itemNetworkObject != null && interactorNetworkObject != null && itemNetworkObject.TryGetComponent<ItemWorld>(out var itemWorld))
        {
            itemWorld.ProcessInteraction(interactorNetworkObject.transform);
        }
    }

    private void ProcessInteraction(Transform interactor)
    {
        InventoryController inventory = interactor.GetComponent<InventoryController>();
        InteractController interact = interactor.GetComponent<InteractController>();
        if (inventory != null)
        {
            Debug.Log(item);
            inventory.SetItem(item);
            interact.RemoveThisInteractable(this);
            DestroySelf();

            InteractController interactController = interactor.GetComponent<InteractController>();
            if (interactController != null)
            {
                interactController.ControlInteractMessage(false);
            }
        }
        else
        {
            Debug.LogError("InventoryController não encontrado no interactor.");
        }
    }
   
    public void DestroySelf()
    {
        if (IsServer)
        {
            NetworkObject netObj = GetComponent<NetworkObject>();
            netObj.Despawn();
        }

        Destroy(gameObject);
    }

}
