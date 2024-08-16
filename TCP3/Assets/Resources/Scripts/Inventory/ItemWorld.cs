using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using CodeMonkey.Utils;

public class ItemWorld : NetworkBehaviour, Interactable
{
    // Método para criar e configurar um ItemWorld com sincronização de rede
    public static void SpawnItemWorld(Vector3 position, Item item)
    {
        if (NetworkManager.Singleton.IsServer) // Verifica se é o servidor
        {
            Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
            ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
            itemWorld.SetItem(item);
            itemWorld.InitializeNetwork();
        }
    }

    private Item item;

    // Configuração do item
    public void SetItem(Item item)
    {
        this.item = item;
        // Instantiate(item.itemModel, transform);
    }

    // Método para criar e lançar o item
    public static void DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomDir = UtilsClass.GetRandomDir();
        Vector3 randomDirX = new Vector3(Random.Range(-.5f, .5f), 1.5f);
        if (NetworkManager.Singleton.IsServer) // Verifica se é o servidor
        {
            ItemWorld itemWorld = Instantiate(ItemAssets.Instance.pfItemWorld, dropPosition + randomDirX * .5f, Quaternion.identity).GetComponent<ItemWorld>();
            itemWorld.SetItem(item);
            itemWorld.InitializeNetwork();
            Rigidbody rb = itemWorld.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(randomDir * .5f, ForceMode.Impulse);
            }
        }
    }

    // Obter o item
    public Item GetItem()
    {
        return item;
    }

    // Destruir o item
    public void DestroySelf()
    {
        if (IsServer) // Verifica se é o servidor
        {
            ItemNetwork(false);
        }
        Destroy(gameObject);
    }

    // Interagir com o item
    public void OnInteract(Transform interactor)
    {
        InventoryController i = interactor.gameObject.GetComponent<InventoryController>();

        if (i != null)
        {
            i.SetItem(item);
            DestroySelf();
            InteractController interactController = interactor.gameObject.GetComponent<InteractController>();
            if (interactController != null)
            {
                interactController.ControlInteractMessage(false);
            }
        }
        else
        {
            Debug.LogError("Não foi encontrado o InventoryController, portanto não rodará o OnInteract");
        }
    }

    // Inicializar a sincronização de rede
    private void InitializeNetwork()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn(); // Chama Spawn apenas no servidor
        }
    }

    // Configurar o estado de rede
    private void ItemNetwork(bool spawn)
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            if (spawn)
            {
                networkObject.Spawn(); // Chama Spawn apenas no servidor
            }
            else
            {
                networkObject.Despawn(); // Chama Despawn apenas no servidor
            }
        }
    }
}
