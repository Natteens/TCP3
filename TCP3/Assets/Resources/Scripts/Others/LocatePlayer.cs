using Unity.Netcode;
using UnityEngine;

public class LocatePlayer : NetworkBehaviour
{
    // Método para obter a posição do jogador (sincronizado)
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
