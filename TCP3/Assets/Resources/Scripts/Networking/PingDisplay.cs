using UnityEngine;
using Unity.Netcode;

public class PingDisplay : MonoBehaviour
{
    void OnGUI()
    {
        // Verifique se o cliente está conectado antes de tentar acessar o ping
        if (NetworkManager.Singleton.IsClient)
        {
            // Obtenha o clientId
            ulong clientId = NetworkManager.Singleton.LocalClientId;

            // Obtenha o ping em milissegundos
            var ping = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(clientId);

            // Exiba o ping na tela
            GUI.Label(new Rect(10, 10, 100, 20), "Ping: " + ping + " ms");
        }
    }
}
