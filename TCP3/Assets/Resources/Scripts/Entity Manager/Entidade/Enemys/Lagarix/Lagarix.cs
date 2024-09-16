using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lagarix : Enemy
{

    public bool canDealDamage = false;

    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Morri");
        DropEnemyItem(GetComponent<EnemySettings>().GetLevel());
        Destroy(gameObject);
    }

    public override void EventActionOnAttack()
    {
        base.EventActionOnAttack();
        canDealDamage = true;
        Destroy(gameObject, 1f);
        canDealDamage = false;
    }

    public override void EventActionOnDeath()
    {
        base.EventActionOnDeath();
        DropEnemyItem(GetComponent<EnemySettings>().GetLevel());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDealDamage && other != null)
        {
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(20);
            }
        }
    }
}
