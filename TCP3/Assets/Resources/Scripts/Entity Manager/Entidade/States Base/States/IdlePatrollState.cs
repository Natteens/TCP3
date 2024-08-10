using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "IdlePatrolState", menuName = "Enemy/States/Idle Patrol State")]
public class IdlePatrolState : EnemyIdleStateSOBase
{
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float patrolPointReachThreshold = 0.5f;
    [SerializeField] private float maxPatrolTime = 3f;

    private Vector2 patrolPoint;
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

        Vector2 moveDirection = (patrolPoint - (Vector2)enemy.transform.position).normalized;
        enemy.Movement(moveDirection);

        float distanceToPatrolPoint = Vector2.Distance(enemy.transform.position, patrolPoint);
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
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        patrolPoint = enemy.transform.position + new Vector3(randomDirection.x, 0f, 0);
    }
}
