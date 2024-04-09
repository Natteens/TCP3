using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private int damage;
    [SerializeField] private float fireRate;

    private bool fireCooldown;
    private List<ParticleCollisionEvent> collisionEvents;
    public Vector3 Target { get; set; }
    public bool Hit { get; set; }

    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    public void Fire()
    {
        if (fireCooldown) return;
        fireCooldown = true;
        particleSystem.Emit(1);
        StartCoroutine(StopCooldownAfterTime());
    }

    private IEnumerator StopCooldownAfterTime()
    {
        yield return new WaitForSeconds(fireRate);
        fireCooldown = false;
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            var collider = collisionEvents[i].colliderComponent;
            Damageable damageable = collider.gameObject.GetComponent<Damageable>();
            if(damageable != null)
            {
                damageable.ApplyDamage(damage);
            }
        }
    }

}
