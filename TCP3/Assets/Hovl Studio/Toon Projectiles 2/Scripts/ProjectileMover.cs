using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DamageNumbersPro;
using System;

public class ProjectileMover : MonoBehaviour
{
    public float Damage;
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public DamageNumber numberPrefab; // Prefab de número de dano
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    private Rigidbody rb;
    public GameObject[] Detached;
    public Transform shooter;
    public LayerMask Layer;

    public void InitializeProjectile(int amount)
    {
        Damage = amount;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ignora a colisão com outros projéteis
        Collider projectileCollider = GetComponent<Collider>();
        ProjectileMover[] allProjectiles = FindObjectsOfType<ProjectileMover>();

        foreach (var projectile in allProjectiles)
        {
            if (projectile != this)
            {
                Collider otherCollider = projectile.GetComponent<Collider>();
                Physics.IgnoreCollision(projectileCollider, otherCollider);
            }
        }

        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }

        Destroy(gameObject, 5); // Destroi após 5 segundos
    }

    void FixedUpdate()
    {
        if (speed != 0)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Lock all axes movement and rotation
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            if ((Layer & (1 << collision.gameObject.layer)) != 0)
            {
                Debug.Log("Colidiu com: " + collision.gameObject.name);

                BaseEntity enemy = collision.gameObject.GetComponent<BaseEntity>();
                if (enemy != null) enemy.lootBuff = Mathf.CeilToInt(shooter.GetComponent<StatusComponent>().GetStatus(StatusType.LootChance) / 10);

                // Caso a entidade morra, dar XP ao atirador do projetil.
                HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();

                if (healthComponent != null)
                {
                    Action onDeathAction = () =>
                    {
                        LevelManager levelManager = shooter.GetComponent<LevelManager>();
                        if (levelManager != null)
                        {
                            levelManager.IncreaseXp(collision.gameObject.GetComponent<EnemySettings>().giveXp);
                        }
                    };

                    // Verifica se a função já está inscrita
                    if (!healthComponent.IsOnDeathSubscribed(onDeathAction))
                    {
                        healthComponent.OnDeath += onDeathAction;
                    }

                    StatusComponent statusComponent = collision.gameObject.GetComponent<StatusComponent>();
                    if (statusComponent != null)
                    {
                        float targetDefense = statusComponent.GetStatus(StatusType.Defense);
                        int finalDamage = DamageCalculator.CalculateWithDefense(targetDefense, Damage);

                        // Aplica o dano
                        healthComponent.TakeDamage(finalDamage);

                        // Instancia e configura o número de dano
                        if (numberPrefab != null)
                        {
                            DamageNumber damageNumber = numberPrefab.Spawn(contact.point, finalDamage);
                            Destroy(damageNumber.gameObject, 2f);
                        }
                    }
                }


            }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
        Spawner.Instance.DespawnInWorld(this.GetComponent<NetworkObject>());
        Destroy(gameObject);
    }
}
