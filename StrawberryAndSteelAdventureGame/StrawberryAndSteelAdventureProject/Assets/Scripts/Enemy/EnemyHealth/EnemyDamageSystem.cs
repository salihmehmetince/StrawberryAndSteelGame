
using System;
using UnityEngine;

public class EnemyDamageSystem
{
    private EnemyHealthSystem healthSystem;

    public bool IsInitialized => healthSystem != null;

    public event Action<int, Vector2> OnDamageReceived;

    public void Initialize(EnemyHealthSystem healthSystem)
    {
        this.healthSystem = null;

        if (healthSystem == null)
        {
            Debug.LogError(
                "EnemyDamageSystem requires an EnemyHealthSystem.");

            return;
        }

        this.healthSystem = healthSystem;
    }

    public int ReceiveDamage(
        int incomingDamage,
        Vector2 attackPosition)
    {
        if (!IsInitialized)
            return 0;

        if (incomingDamage <= 0)
            return 0;

        if (float.IsNaN(attackPosition.x) ||
            float.IsInfinity(attackPosition.x) ||
            float.IsNaN(attackPosition.y) ||
            float.IsInfinity(attackPosition.y))
        {
            return 0;
        }

        int appliedDamage = healthSystem.TakeDamage(incomingDamage);

        if (appliedDamage > 0)
        {
            OnDamageReceived?.Invoke(
                appliedDamage,
                attackPosition);
        }

        return appliedDamage;
    }
}
