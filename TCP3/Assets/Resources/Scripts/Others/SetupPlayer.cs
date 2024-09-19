using System;
using Unity.Netcode;
using UnityEngine;

public class SetupPlayer : NetworkBehaviour
{
    public string ID;
    
    private string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString("N") + "_" + this.gameObject.name.GetHashCode() + "_" + this.gameObject.tag.GetHashCode();
    }

    private void Start()
    {
        if (IsOwner)
        {
            GameManager.Instance.waitForInitialize.SetActive(false);
            ID = GenerateUniqueID();
            PlayersManager.Instance.AddToList(this.gameObject);
            PlayersManager.Instance.SetCurrentId(ID);
            Debug.Log("Player Id gerado corretamente!");
        }
        else
        {
            Debug.LogWarning("Não foi possivel criar o ID pois nao sou o Owner!");
        }
            
    }
}
