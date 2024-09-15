using UnityEngine;

[CreateAssetMenu(fileName = "ChaseState", menuName = "Enemy/States/Chase State")]
public class ChaseState : EnemyChaseStateSOBase
{
    public float stopDistance = 1.5f; // Dist�ncia m�nima para parar

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();

        // Encontrar o jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Calcular a dire��o para o jogador
        Vector3 directionToPlayer = (player.transform.position - enemy.transform.position).normalized;

        // Calcular a dist�ncia at� o jogador
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.transform.position);

        // Se o inimigo est� longe o suficiente, ele se move
        if (distanceToPlayer > stopDistance)
        {
            enemy.Movement(directionToPlayer); // Move o inimigo na dire��o do jogador
        }
        else
        {
            enemy.Movement(Vector3.zero); // Para o movimento se estiver muito pr�ximo
            enemy.ChangeState(enemy.Attack); // Alterna para o estado de ataque
        }
    }
}
