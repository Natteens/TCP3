using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using CodeMonkey.Utils;


public class ItemWorld : NetworkBehaviour, Interactable
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.GetComponent<NetworkObject>().Spawn();
        itemWorld.SetItem(item);

        return itemWorld;
    }

    private Item item;

    public void SetItem(Item item)
    { 
        this.item = item;
        //Instantiate(item.itemModel, transform);
    }

    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomDir = UtilsClass.GetRandomDir();
        Vector3 randomDirX = new Vector3(Random.Range(-.5f, .5f), 1.5f);
        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomDirX * .5f, item);
        itemWorld.GetComponent<Rigidbody>().AddForce(randomDir * .5f, ForceMode.Impulse);
        return itemWorld;
    }

    public Item GetItem() 
    { 
        return item;
    }

    public void DestroySelf()
    { 
        Destroy(gameObject);
        GetComponent<NetworkObject>().Despawn();
    }

    public void OnInteract(Transform interactor)
    {
        InventoryController i = interactor.gameObject.GetComponent<InventoryController>();

        if (i != null)
        {
            i.SetItem(item);
            DestroySelf();
            interactor.gameObject.GetComponent<InteractController>().ControlInteractMessage(false);
        }
        else
        {
            Debug.LogError("Nao foi encontrado o InventoryController portanto nao rodará o OnInteract");
        }
    }
}

