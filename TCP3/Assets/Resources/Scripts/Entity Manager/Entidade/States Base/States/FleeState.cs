using UnityEngine;

[CreateAssetMenu(fileName = "FleeState", menuName = "Enemy/States/Flee State")]
public class FleeState : EnemyChaseStateSOBase
{
    public float SafeDistance = 3f;

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
        Vector2 moveDirection = enemy.ContextSolver.GetDirectionToMove(enemy.SteeringBehaviours, enemy.aiData);

        if (enemy.aiData.currentTarget != null)
        {

            float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.aiData.currentTarget.position);
            if (distanceToTarget <= SafeDistance)
            {
                 enemy.Movement(moveDirection);   
                // mover o inimigo pra outra posição safe
            }
            else
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
