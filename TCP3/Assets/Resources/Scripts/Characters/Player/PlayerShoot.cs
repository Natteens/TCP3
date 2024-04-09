using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private PlayerInputs myInputs;
    [SerializeField] private Gun currentGun;


    void Start()
    {
        Debug.Log(currentGun.GunSpecs.Ammo);

        if (myInputs != null)
        {
            myInputs.ShootAction.performed += _ => currentGun.OnShoot();
        }

        Cursor.lockState = CursorLockMode.Locked;

    }

    private void OnDisable()
    {
        myInputs.ShootAction.performed -= _ => currentGun.OnShoot();
    }

}
