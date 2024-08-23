using System.Collections;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public  StatusComponent statusComponent { get; private set; }
    public  HealthComponent healthComponent { get; private set; }
    public  Knockback knockback { get; private set; }
    public  Animator anim { get; private set; }
    public  Rigidbody rb { get; private set; }
    public  Collider coll { get; private set; }

    public bool IsAlive => healthComponent.IsAlive;

    public virtual void Awake()
    {
        statusComponent = GetComponent<StatusComponent>();
        healthComponent = GetComponent<HealthComponent>();
        knockback = GetComponent<Knockback>();
        anim = GetComponent<Animator>();
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

    private void OnDestroy()
    {
        UnsubscribeFromHealthEvents();
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
    }

    protected virtual void OnTakeDamage(float amount)
    {
        anim.SetTrigger("TakeDamage");
    }

    protected virtual void OnHeal(float amount) { }

    protected virtual void OnRevive()
    {
        coll.enabled = true;
        anim.SetTrigger("Revive");
    }
}
