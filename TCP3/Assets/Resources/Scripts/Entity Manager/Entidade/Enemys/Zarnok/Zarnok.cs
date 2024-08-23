using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zarnok : Enemy
{
    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Morri");
        Destroy(gameObject);
    }

    public override void EventActionOnAttack()
    {
        base.EventActionOnAttack();
        Destroy(gameObject, 1f);
    }
}
