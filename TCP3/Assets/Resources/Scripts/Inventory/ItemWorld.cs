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
        // Cria uma nova inst�ncia do item para evitar problemas de refer�ncia compartilhada
        Item newItem = (Item)ScriptableObject.CreateInstance(item.GetType());
        newItem.Initialize(item);
        this.item = newItem;

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
            // Cria uma nova inst�ncia do item para garantir que estamos manipulando uma c�pia
            Item giveItem = (Item)ScriptableObject.CreateInstance(item.GetType());
            giveItem.Initialize(item);
            inventory.SetItem(giveItem);
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

    // M�todo para instanciar e sincronizar um item no mundo
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
                Debug.Log("N�o sou o server achei o NetOBJ do player e rodei o rpc");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 position, Item item)
    {
        Vector3 finalDropPosition = VariablePosition(position);
        var itemWorldInstance = Instantiate(ItemAssets.Instance.pfItemWorld, finalDropPosition, Quaternion.identity);
        var itemWorld = itemWorldInstance.GetComponent<ItemWorld>();

        // Cria uma nova inst�ncia do item no servidor para evitar compartilhamento de refer�ncia
        Item newItem = (Item)ScriptableObject.CreateInstance(item.GetType());
        newItem.Initialize(item);
        itemWorld.SetItem(newItem);

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
