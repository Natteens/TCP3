using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    public ThirdPersonController player;
    private StarterAssetsInputs starterAssets;
    public CinemachineVirtualCamera aimCamera;

    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    private void Awake()
    {
        player = GetComponent<ThirdPersonController>();
        starterAssets = GetComponent<StarterAssetsInputs>();
        aimCamera = player._AimVirtualCamera;    
    }

    private void Update()
    {
        if (starterAssets.aim)
        {
            aimCamera.gameObject.SetActive(true);
            player.SetSensitivity(aimSensitivity);
        }
        else
        {
            aimCamera.gameObject.SetActive(false);
            player.SetSensitivity(normalSensitivity);
        }
    }
}
