
using UnityEngine;

public class EnemyAttackSystem
{
    private EnemyController enemyController;
    private EnemyAttackCooldownSystem cooldownSystem;

    public bool IsInitialized =>
        enemyController != null &&
        cooldownSystem != null &&
        cooldownSystem.IsInitialized;

    public bool IsOnCooldown =>
        cooldownSystem != null && cooldownSystem.IsOnCooldown;

    public float RemainingCooldown =>
        cooldownSystem != null
            ? cooldownSystem.RemainingCooldown
            : 0f;

    public void Initialize(
        EnemyController enemyController,
        EnemyAttackCooldownSystem cooldownSystem)
    {
        if (enemyController == null || cooldownSystem == null)
        {
            Debug.LogError(
                "EnemyAttackSystem requires an EnemyController and EnemyAttackCooldownSystem.");
            return;
        }

        if (!cooldownSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackSystem requires an initialized cooldown system.");
            return;
        }

        this.enemyController = enemyController;
        this.cooldownSystem = cooldownSystem;
    }

    public bool CanAttack()
    {
        if (!IsInitialized)
            return false;

        if (enemyController.IsDead)
            return false;

        if (cooldownSystem.IsOnCooldown)
            return false;

        return true;
    }

    public bool TryStartAttack()
    {
        if (!CanAttack())
            return false;

        return cooldownSystem.TryStartCooldown();
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized)
            return;

        cooldownSystem.Update(deltaTime);
    }

    public void ResetCooldown()
    {
        if (!IsInitialized)
            return;

        cooldownSystem.ResetCooldown();
    }
}
