using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private BaseGun gunSpecs;
    [SerializeField] private BulletController bullet;

    public void OnShoot()
    {
        bullet.Fire();
    }
}
