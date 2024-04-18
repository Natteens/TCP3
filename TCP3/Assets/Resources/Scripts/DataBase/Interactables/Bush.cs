using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Fruits
{ 
    Orange,
    Blueberry,
    Strawberry
}


public class Bush : MonoBehaviour, Interactable
{

    [SerializeField] private Fruits typeOfFruit;
    [SerializeField] private int quantity;

    public string OnInteract()
    {
        return "+" + quantity.ToString() + " " + typeOfFruit.ToString();
    }

}
