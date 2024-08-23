using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "IdlePatrolState", menuName = "Enemy/States/Idle Patrol State")]
public class IdlePatrollState : EnemyIdleStateSOBase
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

        Debug.Log("Movimentando");
        Vector3 moveDirection = (patrolPoint - enemy.transform.position).normalized;
        enemy.Movement(moveDirection);

        float distanceToPatrolPoint = Vector3.Distance(enemy.transform.position, patrolPoint);
        patrolTimer -= Time.deltaTime;

        if (distanceToPatrolPoint <= patrolPointReachThreshold || patrolTimer <= 0)
        {
            ChoosePatrolPoint();
            patrolTimer = maxPatrolTime;
        }

        if (enemy.aiData.GetTargetsCount() > 0)
        {
            enemy.ChangeState(enemy.Chase);
        }
    }

    private void ChoosePatrolPoint()
    {
        // Generate a random point within the patrol radius in 3D space
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0; // Keep the patrol on the ground plane, assuming y is up
        patrolPoint = enemy.transform.position + randomDirection;
    }
}
