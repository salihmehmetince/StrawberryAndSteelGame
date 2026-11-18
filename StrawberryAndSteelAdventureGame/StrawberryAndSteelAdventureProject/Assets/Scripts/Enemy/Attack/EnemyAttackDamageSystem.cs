
using UnityEngine;

public class EnemyAttackDamageSystem
{
    private int attackDamage;

    public bool IsInitialized { get; private set; }

    public int AttackDamage => attackDamage;

    public void Initialize(float attackDamage)
    {
        if (attackDamage < 0f)
        {
            Debug.LogError(
                "EnemyAttackDamageSystem received an invalid attack damage value.");
            return;
        }

        this.attackDamage = Mathf.RoundToInt(attackDamage);
        IsInitialized = true;
    }

    public bool ApplyDamage(
        IDamageable target,
        Vector2 attackPosition)
    {
        if (!IsInitialized || target == null || attackDamage <= 0)
            return false;

        int appliedDamage = target.ReceiveDamage(
            attackDamage,
            attackPosition);

        return appliedDamage > 0;
    }
}
