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

            // Fugir apenas se o jogador estiver no raio de detecção
            if (distanceToPlayer <= detectionRadius)
            {
                fleeDirection = (npc.transform.position - player.position).normalized;

                // Mover suavemente na direção oposta ao jogador
                Vector3 smoothFleeDirection = Vector3.Slerp(npc.transform.forward, fleeDirection, Time.deltaTime * 5f).normalized;
                npc.Movement(smoothFleeDirection);

                // Rotaciona apenas no eixo Y
                Vector3 lookDirection = new Vector3(smoothFleeDirection.x, 0, smoothFleeDirection.z); // Zera o eixo Y
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
