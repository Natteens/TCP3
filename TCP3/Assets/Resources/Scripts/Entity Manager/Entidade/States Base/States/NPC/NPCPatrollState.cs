using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCPatrolState", menuName = "NPC/States/Patrol State")]
public class NPCPatrollState : IdleStateSOBase
{
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float patrolPointReachThreshold = 0.5f;
    [SerializeField] private float maxPatrolTime = 3f;

    private Vector3 patrolPoint;
    private float patrolTimer;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        ChoosePatrolPoint();
        patrolTimer = maxPatrolTime;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        patrolTimer = 0;
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();

        // Verifica a distância entre o inimigo e o jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.transform.position);

            if (distanceToPlayer <= patrolRadius) // Se o jogador estiver perto, mudar para o estado de perseguição
            {
                enemy.ChangeState(enemy.Chase);
                return;
            }
        }

        // Código de patrulha continua...
        Vector3 moveDirection = (patrolPoint - enemy.transform.position).normalized;
        enemy.Movement(moveDirection);

        float distanceToPatrolPoint = Vector3.Distance(enemy.transform.position, patrolPoint);
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
        patrolPoint = enemy.transform.position + randomDirection;
    }
}
