using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseEntity : NetworkBehaviour
{
    public  StatusComponent statusComponent { get; private set; }
    public  HealthComponent healthComponent { get; private set; }
    public  Knockback knockback { get; private set; }
    public  Animator anim { get; private set; }
    public  Rigidbody rb { get; private set; }
    public  Collider coll { get; private set; }

    public int lootBuff = 0;

    public bool IsAlive => healthComponent.IsAlive;

    public virtual void Awake()
    {
        statusComponent = GetComponent<StatusComponent>();
        healthComponent = GetComponent<HealthComponent>();
        knockback = GetComponent<Knockback>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        SubscribeToHealthEvents();
    }

    protected virtual void SubscribeToHealthEvents()
    {
        healthComponent.OnDeath += OnDeath;
        healthComponent.OnTakeDamage += OnTakeDamage;
        healthComponent.OnHeal += OnHeal;
        healthComponent.OnRevive += OnRevive;
    }

    protected virtual void UnsubscribeFromHealthEvents()
    {
        healthComponent.OnDeath -= OnDeath;
        healthComponent.OnTakeDamage -= OnTakeDamage;
        healthComponent.OnHeal -= OnHeal;
        healthComponent.OnRevive -= OnRevive;
    }

    protected virtual void OnDeath()
    {
        anim.SetTrigger("Death");
        coll.enabled = false;
        rb.isKinematic = true;
        Destroy(gameObject);
        UnsubscribeFromHealthEvents();
    }

    protected virtual void GiveXp(int xpAmount, Transform killer) { }

    protected virtual void OnTakeDamage(float amount)
    {
        anim.SetTrigger("TakeDamage");
    }

    protected virtual void OnHeal(float amount) { }

    protected virtual void OnRevive()
    {
        coll.enabled = true;
        rb.isKinematic = false;
        anim.SetTrigger("Revive");
        SubscribeToHealthEvents();
    }

    public void MoveAndRotate(Vector3 dir, float speed)
    {
        Vector3 normalizedDirection = new Vector3(dir.x, 0, dir.z).normalized;
        Vector3 movement = normalizedDirection * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        if (normalizedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(normalizedDirection);
            Quaternion newRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * speed));
        }

        float blendSpeed = (speed == 0) ? 0f : (speed > 0 && speed < 3) ? 0.5f : 1f;
        anim.SetFloat("Speed", blendSpeed);
    }
}
