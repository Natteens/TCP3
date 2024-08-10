public interface IHealth
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    bool IsAlive { get; }

    void TakeDamage(float amount);
    void Heal(float amount);
    void Die();
    void Revive();
}
