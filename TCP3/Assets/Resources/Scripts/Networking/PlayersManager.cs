using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : Singleton<PlayersManager>
{
    [SerializeField] private List<GameObject> playersList;
    [SerializeField] private string currentPlayerId;
    public List<GameObject> PlayersList { get { return playersList; } }
    public string CurrentPlayerId { get { return currentPlayerId; } }

    public void AddToList(GameObject obj)
    { 
        playersList.Add(obj);
    }

    public void SetCurrentId(string newId)
    {
        currentPlayerId = newId;
    }

    public void RemoveFromList(GameObject obj)
    {
        playersList.Remove(obj);
    }

    public GameObject GetMyPlayer()
    {
        foreach (GameObject player in playersList)
        {
            if (player.GetComponent<SetupPlayer>().ID == currentPlayerId)
                return player;
        }

        return null;
    }

    public GameObject GetPlayerById(string specificID)
    {
        foreach (GameObject player in playersList)
        {
            if (player.GetComponent<SetupPlayer>().ID == specificID)
                return player;
        }

        return null;
    }
}
