
using System;

public class EnemyHealthSystem
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public float Defense { get; private set; }
    public bool IsDead { get; private set; }

    public float HealthPercentage =>
        MaxHealth > 0
            ? (float)CurrentHealth / MaxHealth
            : 0f;

    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnDamaged;
    public event Action<int> OnHealed;
    public event Action OnDied;

    public EnemyHealthSystem(int maxHealth, float defense)
    {
        MaxHealth = maxHealth > 0 ? maxHealth : 1;

        Defense = !float.IsNaN(defense) &&
                  !float.IsInfinity(defense) &&
                  defense > 0f
            ? defense
            : 0f;

        CurrentHealth = MaxHealth;
        IsDead = false;
    }

    public int TakeDamage(int incomingDamage)
    {
        if (IsDead || incomingDamage <= 0)
            return 0;

        double calculatedDamage = Math.Max(
            0d,
            incomingDamage - (double)Defense);

        int actualDamage = (int)Math.Min(
            int.MaxValue,
            calculatedDamage);

        int previousHealth = CurrentHealth;

        CurrentHealth = Math.Max(
            0,
            CurrentHealth - actualDamage);

        int appliedDamage = previousHealth - CurrentHealth;

        if (appliedDamage <= 0)
            return 0;

        if (CurrentHealth <= 0)
            IsDead = true;

        OnDamaged?.Invoke(appliedDamage);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead)
            OnDied?.Invoke();

        return appliedDamage;
    }

    public int Heal(int healAmount)
    {
        if (IsDead || healAmount <= 0)
            return 0;

        int previousHealth = CurrentHealth;

        CurrentHealth = (int)Math.Min(
            MaxHealth,
            (long)CurrentHealth + healAmount);

        int healedAmount = CurrentHealth - previousHealth;

        if (healedAmount <= 0)
            return 0;

        OnHealed?.Invoke(healedAmount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        return healedAmount;
    }
}
