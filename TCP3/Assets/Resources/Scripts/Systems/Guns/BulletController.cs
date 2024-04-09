using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private int Damage = 2;
    [SerializeField] private GameObject bulletDecal;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float timeToDestroy = 3f;

    public Vector3 Target { get; set; }
    public bool Hit { get; set; }

    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target, bulletSpeed * Time.deltaTime);
        if (!Hit && Vector3.Distance(transform.position, Target) < 0.01f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 contactPoint = other.transform.position;

        GameObject.Instantiate(bulletDecal, contactPoint, Quaternion.LookRotation(Vector3.up));

        Damageable damageable = other.gameObject.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(Damage);
        }

        Destroy(gameObject);
    }

}
