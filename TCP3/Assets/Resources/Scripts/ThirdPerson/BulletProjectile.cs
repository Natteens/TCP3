using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        float speed = 40f;
        bulletRigidbody.velocity = transform.forward * speed;
        Destroy(gameObject,3f);
    }

    private void OnTriggerEnter(Collider other)
    {
<<<<<<< Updated upstream
        if (other.GetComponent<BulletTarget>() != null)
        {
            //hit target
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        }
        else
        {
            //hit something else
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
=======
        Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        Destroy(gameObject);
>>>>>>> Stashed changes
    }
}
