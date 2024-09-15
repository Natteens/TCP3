using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Enemy/States/Ranged Basic Attack State")]
public class RangedBasicAttackState : EnemyAttackStateSOBase
{
    [SerializeField] private float minAttackRange = 1.5f;
    [SerializeField] private float maxAttackRange = 4f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 10f;
    [SerializeField] private float projectileRange = 5f;

    private bool isOnCooldown = false;
    private float cooldownTimer = 0f; // Timer para gerenciar cooldown

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        isOnCooldown = false;
        cooldownTimer = attackCooldown; // Reseta o cooldown
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.transform.position);

        // Se o inimigo est� no alcance para atacar
        if (distanceToPlayer <= maxAttackRange)
        {
            // Se n�o estiver em cooldown, atira
            if (!isOnCooldown)
            {
                Attack(player);
                isOnCooldown = true;
            }
        }
        // Se o jogador sair do alcance, mudar para o estado de persegui��o
        else if (distanceToPlayer > maxAttackRange)
        {
            enemy.ChangeState(enemy.Chase);
        }

        // Se estiver em cooldown, atualiza o timer
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = attackCooldown; // Reseta o cooldown
            }
        }
    }

    private void Attack(GameObject player)
    {
        // Ativar anima��o de ataque (isShooting)
        enemy.anim.SetTrigger("isShooting");
        enemy.Movement(Vector2.zero);
        RotateFirePointTowardsTarget(player.transform);
        FireProjectile();
    }

    private void RotateFirePointTowardsTarget(Transform target)
    {
        if (target != null && enemy.firePoint != null)
        {
            Vector3 direction = target.position - enemy.transform.position;
            direction.y = 0; // Garante que o inimigo s� rotacione no plano XZ
            enemy.firePoint.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void FireProjectile()
    {
        if (enemy.firePoint != null && projectilePrefab != null)
        {
            ulong shooterId = enemy.GetComponent<NetworkObject>().NetworkObjectId;
            Vector3 shootDirection = enemy.firePoint.forward;
            Spawner.Instance.SpawnProjectilesServerRpc(enemy.firePoint.position, shootDirection, "projectileBasic", 20, shooterId);
        }
    }
}
