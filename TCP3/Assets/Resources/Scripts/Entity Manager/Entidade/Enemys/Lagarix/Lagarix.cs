using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lagarix : Enemy
{
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
        Destroy(gameObject, 1f);
    }

    public override void EventActionOnDeath()
    {
        base.EventActionOnDeath();
        DropEnemyItem(GetComponent<EnemySettings>().GetLevel());
    }


}
