using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocatePlayer : MonoBehaviour
{
    //Script para dar a referencia do jogador
    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }
}
