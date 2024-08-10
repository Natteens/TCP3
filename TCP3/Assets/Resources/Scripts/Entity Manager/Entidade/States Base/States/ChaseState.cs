using UnityEngine;

[CreateAssetMenu(fileName = "ChaseState", menuName = "Enemy/States/Chase State")]
public class ChaseState : EnemyChaseStateSOBase
{
    public float StopDistance = 1.5f;

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
        Vector2 moveDirection = enemy.ContextSolver.GetDirectionToMove(enemy.SteeringBehaviours, enemy.aiData);
        enemy.Movement(moveDirection);

        if (enemy.aiData.currentTarget != null)
        {

            float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.aiData.currentTarget.position);
            if (distanceToTarget <= StopDistance)
            {
                enemy.ChangeState(enemy.Attack);
            }
        }
        else
        {
            enemy.ChangeState(enemy.Idle);
        }
    }
}
