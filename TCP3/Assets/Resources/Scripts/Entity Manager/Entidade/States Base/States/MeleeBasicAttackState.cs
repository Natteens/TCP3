using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Enemy/States/Melee Basic Attack State")]
public class MeleeBasicAttackState : EnemyAttackStateSOBase
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.0f;

    private bool isOnCooldown;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        isOnCooldown = false;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (enemy.aiData.currentTarget != null)
        {
            float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.aiData.currentTarget.position);
            if (distanceToTarget <= attackRange && !isOnCooldown)
            {
                FlipTowardsTarget(enemy.aiData.currentTarget);
                enemy.StartCoroutine(Attack());
            }
            else if (distanceToTarget > attackRange)
            {
                enemy.ChangeState(enemy.Chase);
            }
        }
        else
        {
            enemy.ChangeState(enemy.Idle);
        }
    }

    private void FlipTowardsTarget(Transform target)
    {
        Vector3 targetPosition = target.position;
        if (targetPosition.x < transform.position.x)
        {
            enemy.attackCollider.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else
        {
            enemy.attackCollider.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private IEnumerator Attack()
    {
        enemy.Movement(Vector2.zero);
        isOnCooldown = true;
        enemy.anim.SetTrigger("MeleeAttack");
        yield return new WaitUntil(() => enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack"));
        yield return new WaitUntil(() => enemy.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }
}
