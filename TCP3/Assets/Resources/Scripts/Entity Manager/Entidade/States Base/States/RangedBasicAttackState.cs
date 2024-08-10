using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Enemy/States/Ranged Basic Attack State")]
public class RangedBasicAttackState : EnemyAttackStateSOBase
{

    [SerializeField] private float minAttackRange = 1.5f;
    [SerializeField] private float maxAttackRange = 4f;
 //   [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileRange = 5f;

    private bool isOnCooldown = false;

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

            RangeToAttack(distanceToTarget);
        }
        else
        {
            enemy.ChangeState(enemy.Idle);
        }
    }

    private void RangeToAttack(float distanceToTarget)
    {
        if (enemy.attackRangeInUse)
        {
            if (distanceToTarget <= minAttackRange && !isOnCooldown)
            {
                RotateFirePointTowardsTarget(enemy.aiData.currentTarget);
                enemy.StartCoroutine(Attack());
            }
            else if (distanceToTarget > minAttackRange)
            {
                enemy.ChangeState(enemy.Chase);
            }
        }
        else
        {
            if (distanceToTarget <= maxAttackRange && !isOnCooldown)
            {
                RotateFirePointTowardsTarget(enemy.aiData.currentTarget);
                enemy.StartCoroutine(Attack());
            }
            else if (distanceToTarget > maxAttackRange)
            {
                enemy.ChangeState(enemy.Chase);
            }
        }
    }

    private void RotateFirePointTowardsTarget(Transform target)
    {
        if (target != null && enemy.firePoint != null)
        {
            Vector2 direction = target.position - enemy.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            enemy.firePoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private IEnumerator Attack()
    {
        enemy.Movement(Vector2.zero);
        isOnCooldown = true;
        enemy.anim.SetTrigger("RangedAttack");
        yield return new WaitUntil(() => enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("RangedAttack"));
        yield return new WaitUntil(() => enemy.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    private void FireProjectile()
    {
        if (enemy.firePoint != null)
        {
          //  Projectile projectile = Instantiate(projectilePrefab, enemy.firePoint.position, enemy.firePoint.rotation);
         //   projectile.UpdateProjectile(projectileRange, projectileSpeed, projectileDamage);
         //   projectile.GetComponent<EnemyDamageSource>().attacker = enemy;
        }
    }

    public override void EventOnAttackAnimationIn()
    {
        FireProjectile();
    }
}
