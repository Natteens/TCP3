using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCFleeState", menuName = "NPC/States/Flee State")]
public class NPCFleeState : ChaseStateSOBase
{
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float safeDistance = 6f;
    [SerializeField] private float playerChaseSpeed = 3f;

    private Transform player;
    private Vector3 fleeDirection;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                fleeDirection = (npc.transform.position - player.position).normalized;
                Flee();
            }
        }
    }

    private void Flee()
    {
        npc.Movement(fleeDirection);
    }
}
