using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;

public class SwitchVCam : MonoBehaviour
{
    private int priorityBoostAmount = 10;

    public static SwitchVCam Instance { get; private set; }

    [SerializeField] private PlayerInputs myInputs;

    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineVirtualCamera vcamthird;
    [SerializeField] private Canvas thirdPersonCanvas;
    [SerializeField] private Canvas aimCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    private void Update()
    {
        CameraControlDisabled();
    }

    private void OnDisable()
    {
        if (myInputs != null)
        {
            myInputs.AimAction.performed -= _ => StartAim();
            myInputs.AimAction.canceled -= _ => CancelAim();
        }      
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

    public void SetInputs(PlayerInputs input)
    {
        myInputs = input;

        if (myInputs != null)
        {
            myInputs.AimAction.performed += _ => StartAim();
            myInputs.AimAction.canceled += _ => CancelAim();
        }    
    }

    public void CameraControlDisabled()
    {
        //Estou na terceira pessoa
        if (aimCanvas.enabled == false)
        {
            vcam.ForceCameraPosition(vcamthird.gameObject.transform.position,
                vcamthird.gameObject.transform.rotation);
            
        }

        //Estou na mira
        if (thirdPersonCanvas.enabled == false)
        {
            vcamthird.ForceCameraPosition(vcam.gameObject.transform.position,
                vcam.gameObject.transform.rotation);
        }
    }

}
