using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class LazerProjectil : NetworkBehaviour
{
    public GameObject vfxHit;
    public float speed = 10f;
    public float vfxHitLifetime = 1.5f;
    public float projectileLifetime = 5f;

    private Rigidbody rb;
    private float creationTime; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        MoveProjectile();
        creationTime = Time.time; 
    }

    private void Update()
    {
        if (Time.time > creationTime + projectileLifetime)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision C)
    {
        speed = 0f;
        ContactPoint contact = C.contacts[0];
        Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        Instantiate(vfxHit, pos, rot);
        Destroy(gameObject);     
        Destroy(vfxHit, vfxHitLifetime);
    }

}
