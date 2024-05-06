using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    string OnInteract();
    bool Giver();
    BaseItem AddItem();
    int ItemQuantity();
}
