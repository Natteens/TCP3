using UnityEngine;
using Unity.Netcode;
using CodeMonkey.Utils;
using EasyBuildSystem.Packages.Addons.AdvancedBuilding;

public class ItemWorld : NetworkBehaviour, Interactable
{
    private Item item;

    // M�todo para definir o item
    public void SetItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null in SetItem!");
            return;
        }

        //Debug.Log("Setting item with amount: " + item.amount);
        this.item = item;
    }

    // M�todo para obter o item
    public Item GetItem()
    {
        return item;
    }

    // M�todo para interagir com o item
    public void OnInteract(Transform interactor)
    {
        if (IsServer)
        {
            // Se j� estiver no servidor, processa a intera��o diretamente
            ProcessInteraction(interactor);
        }
        else
        {
            // Caso contr�rio, chama o ServerRpc para processar a intera��o no servidor
            OnInteractClientRpc(NetworkObject.NetworkObjectId, interactor.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private void ProcessInteraction(Transform interactor)
    {
        InventoryController inventory = interactor.GetComponent<InventoryController>();
        InteractController interact = interactor.GetComponent<InteractController>();
        if (inventory != null)
        {
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
            Debug.LogError("InventoryController n�o encontrado no interactor.");
        }
    }

    public static void SpawnItemWorld(Vector3 position, Item item)
    {
        Vector3 finalDropPosition = VariablePosition(position);
        var itemWorldInstance = Instantiate(ItemAssets.Instance.pfItemWorld, finalDropPosition, Quaternion.identity);
        itemWorldInstance.GetComponent<ItemWorld>().SetItem(item);

        NetworkObject networkObject = itemWorldInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); // Sincroniza o objeto com todos os clientes
        }
    }

    public static void DropItem(Vector3 dropPosition, Item item)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log(item.amount);
            SpawnItemWorld(dropPosition, item);
            Debug.Log("sou o servidor to chamando o Spawn!");
        }
        else
        {
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.GetComponent<ItemWorld>().DropItemServerRpc(dropPosition, item);
                Debug.Log("N�o sou o server achei o NetOBJ do player e rodei o rpc");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 position, Item item)
    {
        Debug.Log(item.amount);
        Vector3 finalDropPosition = VariablePosition(position);
        var itemWorldInstance = Instantiate(ItemAssets.Instance.pfItemWorld, finalDropPosition, Quaternion.identity);
        var itemWorld = itemWorldInstance.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        NetworkObject networkObject = itemWorldInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }

    private static Vector3 VariablePosition(Vector3 dropPosition)
    {
        int dropRadius = 3;
        Vector3 randomOffset = new Vector3(Random.Range(-dropRadius, dropRadius), 0, Random.Range(-dropRadius, dropRadius));
        Vector3 finalDropPosition = dropPosition + randomOffset;
        return finalDropPosition;
    }

    // M�todo para destruir o item
    public void DestroySelf()
    {
        if (IsServer)
        {
            NetworkObject netObj = GetComponent<NetworkObject>();
            netObj.Despawn();
        }

        Destroy(gameObject);
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
}
