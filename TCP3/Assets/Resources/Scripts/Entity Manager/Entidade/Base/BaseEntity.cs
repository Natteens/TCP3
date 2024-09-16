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
        // Normaliza a direção do movimento no plano XZ
        Vector3 normalizedDirection = new Vector3(dir.x, 0, dir.z).normalized;

        // Calcula o movimento baseado na direção e na velocidade
        Vector3 movement = normalizedDirection * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z); // Mantém a velocidade vertical

        // Rotação suave se estiver se movendo
        if (normalizedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(normalizedDirection);
            Quaternion newRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10f)); // Aumenta a suavidade da rotação
        }

        // Verifica a velocidade real do rigidbody no plano XZ
        float velocityMagnitude = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        // Corrige o blendSpeed da animação com base na magnitude da velocidade
        if (velocityMagnitude < 0.1f) // Parado
        {
            anim.SetFloat("Speed", 0f);
        }
        else if (velocityMagnitude < 3f) // Andando
        {
            anim.SetFloat("Speed", 0.5f);
        }
        else // Correndo
        {
            anim.SetFloat("Speed", 1f);
        }
    }

}
