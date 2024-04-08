using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private PlayerInputs myInputs;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private float bulletHitMissDistance = 25f;

    void Start()
    {
        cameraTransform = Camera.main.transform;

        if (myInputs != null)
        {
            myInputs.ShootAction.performed += _ => ShootGun();
        }

        Cursor.lockState = CursorLockMode.Locked;
            
    }

    private void OnDisable()
    {
        myInputs.ShootAction.performed -= _ => ShootGun();
    }

    private void ShootGun()
    { 
        RaycastHit hit;
        float gunRange = 200f;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>(); //usar interface aq

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, gunRange))
        {
            bulletController.Target = hit.point;
            bulletController.Hit = true;
        }
        else
        {
            bulletController.Target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
            bulletController.Hit = false;
        }
    }
}
