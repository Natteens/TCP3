using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DamageNumbersPro;

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
        // Trava o movimento e a rotação do projétil após a colisão
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            var hitInstance = Instantiate(hit, pos, rot);
            if (UseFirePointRotation)
            {
                hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
            }
            else if (rotationOffset != Vector3.zero)
            {
                hitInstance.transform.rotation = Quaternion.Euler(rotationOffset);
            }
            else
            {
                hitInstance.transform.LookAt(contact.point + contact.normal);
            }

            // Se colidiu com um objeto que faz parte da camada definida
            if ((Layer & (1 << collision.gameObject.layer)) != 0)
            {
                Debug.Log("Colidiu com: " + collision.gameObject.name);
                HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();
                StatusComponent status = collision.gameObject.GetComponent<StatusComponent>();

                if (health != null && status != null)
                {
                    // Cálculo do dano com defesa
                    float targetDefense = status.GetStatus(StatusType.Defense);
                    int finalDamage = DamageCalculator.CalculateWithDefense(targetDefense, Damage);

                    // Aplica o dano à saúde do alvo
                    health.TakeDamage(finalDamage);

                    // Instancia o número de dano
                    if (numberPrefab != null)
                    {
                        DamageNumber damageNumber = numberPrefab.Spawn(pos, finalDamage);
                    }

                    // Caso a entidade morra, o atirador ganha XP
                    if (health.CurrentHealth <= 0)
                    {
                        shooter.GetComponent<LevelManager>().IncreaseXp(collision.gameObject.GetComponent<EnemySettings>().giveXp);
                    }
                }
            }

            // Destroi a hit instance após o término da partícula
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

        // Solta os objetos "Detached"
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }

        // Despawning e destruição do projétil
        Spawner.Instance.DespawnInWorld(this.GetComponent<NetworkObject>());
        Destroy(gameObject);
    }
}
