using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private PlayerInputs myInputs;
    [SerializeField] private Gun currentGun;


    void Start()
    {
        if (IsOwner)
        {
            if (myInputs != null)
            {
                myInputs.ShootAction.performed += _ => currentGun.OnShoot();
            }

        }
    }

    private void OnDisable()
    {
        if (IsOwner)
        {
            myInputs.ShootAction.performed -= _ => currentGun.OnShoot();
        }     
    }

}
