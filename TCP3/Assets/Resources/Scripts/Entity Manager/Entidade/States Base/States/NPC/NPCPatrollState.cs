using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCPatrolState", menuName = "NPC/States/Patrol State")]
public class NPCPatrollState : IdleStateSOBase
{
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float patrolPointReachThreshold = 0.5f;
    [SerializeField] private float maxPatrolTime = 3f;
    [SerializeField] private float detectionRadius = 10f; // Raio para detectar o jogador

    private Vector3 patrolPoint;
    private float patrolTimer;
    private Transform player;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        ChoosePatrolPoint();
        patrolTimer = maxPatrolTime;
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Assume que o jogador tem a tag "Player"
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        patrolTimer = 0;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();

        // Verifica se o jogador está no range de detecção
        if (player != null && Vector3.Distance(npc.transform.position, player.position) <= detectionRadius)
        {
            // Troca para o estado de fuga
            npc.ChangeState(npc.Chase);
            return; // Não continua a lógica de patrulha se o jogador estiver no range
        }

        // Lógica de patrulha
        Vector3 moveDirection = (patrolPoint - npc.transform.position).normalized;
        npc.Movement(moveDirection);

        float distanceToPatrolPoint = Vector3.Distance(npc.transform.position, patrolPoint);
        patrolTimer -= Time.deltaTime;

        if (distanceToPatrolPoint <= patrolPointReachThreshold || patrolTimer <= 0)
        {
            ChoosePatrolPoint();
            patrolTimer = maxPatrolTime;
        }
    }

    private void ChoosePatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0; // Para evitar que o NPC se mova no eixo Y
        patrolPoint = npc.transform.position + randomDirection;
    }
}
