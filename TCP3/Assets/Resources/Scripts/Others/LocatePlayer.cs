using Unity.Netcode;
using UnityEngine;

public class LocatePlayer : NetworkBehaviour
{
    // M�todo para obter a posi��o do jogador (sincronizado)
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
