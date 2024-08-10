using System.Collections;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public  StatusComponent statusComponent { get; private set; }
    public  HealthComponent healthComponent { get; private set; }
    public Knockback knockback { get; private set; }
    public  Animator anim { get; private set; }
    public  SpriteRenderer spriteRenderer { get; private set; }
    public  Rigidbody2D rb { get; private set; }
    public  Collider2D coll { get; private set; }
    public  Material originalMaterial { get; private set; }

    public bool IsAlive => healthComponent.IsAlive;

    public virtual void Awake()
    {
        statusComponent = GetComponent<StatusComponent>();
        healthComponent = GetComponent<HealthComponent>();
        knockback = GetComponent<Knockback>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        originalMaterial = spriteRenderer.material;

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
        //rb.bodyType = RigidbodyType2D.Kinematic;
        coll.enabled = false;
    }

    protected virtual void OnTakeDamage(float amount)
    {
        anim.SetTrigger("TakeDamage");
        BlinkWhite();
    }

    protected virtual void OnHeal(float amount) { }

    protected virtual void OnRevive()
    {
        coll.enabled = true;
       // rb.bodyType = RigidbodyType2D.Dynamic;
        anim.SetTrigger("Revive");
    }

    public void BlinkWhite()
    {
        ApplyBlinkEffect(spriteRenderer, 0.1f);
    }

    public void ApplyBlinkEffect(SpriteRenderer spriteRenderer, float duration = 0.125f)
    {
        originalMaterial = spriteRenderer.material;
        Material blinkMaterial = spriteRenderer.material;
        blinkMaterial.SetFloat("_BlinkAmount", 1f);
        StartCoroutine(RestoreOriginalMaterialAfterDelay(spriteRenderer, originalMaterial, duration));
    }

    public IEnumerator RestoreOriginalMaterialAfterDelay(SpriteRenderer spriteRenderer, Material originalMaterial, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (originalMaterial != null)
        {
            spriteRenderer.material = originalMaterial;
            originalMaterial.SetFloat("_BlinkAmount", 0f);
        }
    }
}
