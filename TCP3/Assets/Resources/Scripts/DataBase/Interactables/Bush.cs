using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Fruits
{ 
    Orange,
    Pineapple,
    Strawberry
}


public class Bush : MonoBehaviour, Interactable
{
    [SerializeField] private BaseItem item;
    [SerializeField] private int quantity;

    public string OnInteract()
    {
        //Mensagem
        return "+" + quantity.ToString() + " " + item.ItemName;
    }

    public bool Giver() { return true; }
    public BaseItem AddItem() { return item; }
    public int ItemQuantity() { return quantity; }


}
