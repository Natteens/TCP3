using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private BaseGun gunSpecs;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform cameraTransform;

    public BaseGun GunSpecs { get { return gunSpecs; } }

    public void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    public void OnShoot()
    {
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(gunSpecs.BulletPrefab, barrelTransform.position, Quaternion.identity);
        BulletController bulletController = bullet.GetComponent<BulletController>(); //usar interface aq

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, gunSpecs.GunRange))
        {
            bulletController.Target = hit.point;
            bulletController.Hit = true;
        }
        else
        {
            bulletController.Target = cameraTransform.position + cameraTransform.forward * 25f;
            bulletController.Hit = false;
        }
    }
}
