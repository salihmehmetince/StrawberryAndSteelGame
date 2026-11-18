
using UnityEngine;

public class EnemyAttackValidationSystem
{
    private EnemyAttackSystem attackSystem;
    private EnemyAttackTargetSystem targetSystem;
    private EnemyAttackLineOfSightSystem lineOfSightSystem;

    public bool IsInitialized =>
        attackSystem != null &&
        targetSystem != null &&
        lineOfSightSystem != null &&
        attackSystem.IsInitialized &&
        targetSystem.IsInitialized &&
        lineOfSightSystem.IsInitialized;

    public void Initialize(
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackLineOfSightSystem lineOfSightSystem)
    {
        if (attackSystem == null ||
            targetSystem == null ||
            lineOfSightSystem == null)
        {
            Debug.LogError(
                "EnemyAttackValidationSystem requires all attack systems.");
            return;
        }

        if (!attackSystem.IsInitialized ||
            !targetSystem.IsInitialized ||
            !lineOfSightSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackValidationSystem received an uninitialized system.");
            return;
        }

        this.attackSystem = attackSystem;
        this.targetSystem = targetSystem;
        this.lineOfSightSystem = lineOfSightSystem;
    }

    public bool CanAttack()
    {
        if (!IsInitialized)
            return false;

        if (!targetSystem.HasValidTarget())
            return false;

        if (!targetSystem.IsTargetInRange)
            return false;

        if (!lineOfSightSystem.HasLineOfSight(
            targetSystem.CurrentTarget))
        {
            return false;
        }

        return attackSystem.CanAttack();
    }
}
