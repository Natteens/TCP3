using Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour
{
    [SerializeField] Transform PlayerCamRoot;

    private void Start()
    {
        NetworkObject thisObject = GetComponent<NetworkObject>();

        if (thisObject.HasStateAuthority)
        {
            GameObject virtualCamera = GameObject.Find("PlayerFollowCamera");
            virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = PlayerCamRoot;
        }
    }
}
