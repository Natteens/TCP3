using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SwitchVCam : MonoBehaviour
{
    private int priorityBoostAmount = 10;

    [SerializeField] private PlayerInputs myInputs;
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Canvas thirdPersonCanvas;
    [SerializeField] private Canvas aimCanvas;

    void Start()
    {
        if (myInputs != null)
        {
            myInputs.AimAction.performed += _ => StartAim();
            myInputs.AimAction.canceled += _ => CancelAim();
        }
    }

    private void OnDisable()
    {
        myInputs.AimAction.performed -= _ => StartAim();
        myInputs.AimAction.canceled  -= _ => CancelAim();
    }

    private void StartAim()
    {
        vcam.Priority += priorityBoostAmount;   
        aimCanvas.enabled = true;
        thirdPersonCanvas.enabled = false;
    }

    private void CancelAim()
    {
        vcam.Priority -= priorityBoostAmount;
        aimCanvas.enabled = false;
        thirdPersonCanvas.enabled = true;
    }
}
