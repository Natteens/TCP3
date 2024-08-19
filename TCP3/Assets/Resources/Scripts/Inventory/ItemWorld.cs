using UnityEngine;
using Unity.Netcode;
using CodeMonkey.Utils;

public class ItemWorld : NetworkBehaviour, Interactable
{
    private Item item;

    // Método para definir o item
    public void SetItem(Item item)
    {
        this.item = item;
    }

    // Método para obter o item
    public Item GetItem()
    {
        return item;
    }

    // Método para interagir com o item
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
            OnInteractServerRpc(NetworkObject.NetworkObjectId, interactor.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private void ProcessInteraction(Transform interactor)
    {
        InventoryController inventory = interactor.GetComponent<InventoryController>();
        if (inventory != null)
        {
            inventory.SetItem(item);
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

    // Método para instanciar e sincronizar um item no mundo
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
            SpawnItemWorld(dropPosition, item);
            Debug.Log("sou o servidor to chamando o Spawn!");
        }
        else
        {
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.GetComponent<ItemWorld>().DropItemServerRpc(dropPosition, item);
                Debug.Log("Não sou o server achei o NetOBJ do player e rodei o rpc");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 position, Item item)
    {
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

    // Método para destruir o item
    public void DestroySelf()
    {
        if (IsServer)
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn();
            }
        }
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnInteractServerRpc(ulong itemNetworkObjectId, ulong interactorNetworkObjectId)
    {
        NetworkObject itemNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[itemNetworkObjectId];
        NetworkObject interactorNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[interactorNetworkObjectId];

        if (itemNetworkObject != null && interactorNetworkObject != null && itemNetworkObject.TryGetComponent<ItemWorld>(out var itemWorld))
        {
            itemWorld.ProcessInteraction(interactorNetworkObject.transform);
        }
    }
}
