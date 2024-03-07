using Unity.Netcode;
using UnityEngine;

public class StartConection : MonoBehaviour
{

   public void StartHostButton()
   {
        GetComponent<NetworkManager>().StartHost();
   }

   public void StartClientButton()
   {
        GetComponent<NetworkManager>().StartClient();
   }
}
