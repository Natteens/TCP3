using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CamTarget : MonoBehaviour
{
    [SerializeField] Transform PlayerCamRoot;

    private void Start()
    {
        NetworkObject thisObject = GetComponent<NetworkObject>();

        if (thisObject.IsOwner)
        {
            GameObject virtualCamera = GameObject.Find("PlayerFollowCamera");
            virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = PlayerCamRoot;

            GetComponent<ThirdPersonController>().enabled = true;
        }
    }
}
