using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraSetTarget : NetworkBehaviour
{
    [SerializeField] private Transform root;
    private CinemachineVirtualCamera thirdCamera;
    private CinemachineVirtualCamera aimCamera;

    private void Awake()
    {
        thirdCamera = GameObject.Find("3rdPersonCinemachine").
            GetComponent<CinemachineVirtualCamera>();

        aimCamera = GameObject.Find("AimCinemachine").
            GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        if (IsOwner)
        {
            thirdCamera.Follow = root;
            thirdCamera.LookAt = root;
            aimCamera.Follow = root;
            aimCamera.LookAt = root;
        }
    }

}
