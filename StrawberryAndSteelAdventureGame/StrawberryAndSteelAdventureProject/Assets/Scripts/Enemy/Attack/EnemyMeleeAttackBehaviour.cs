
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackBehaviour : IEnemyAttackBehaviour
{
    private EnemyAttackSystem attackSystem;
    private EnemyAttackTargetSystem targetSystem;
    private EnemyAttackDamageSystem damageSystem;
    private EnemyAttackValidationSystem validationSystem;
    private EnemyMeleeHitDetectionSystem hitDetectionSystem;

    public bool IsInitialized =>
        attackSystem != null &&
        targetSystem != null &&
        damageSystem != null &&
        validationSystem != null &&
        hitDetectionSystem != null &&
        attackSystem.IsInitialized &&
        targetSystem.IsInitialized &&
        damageSystem.IsInitialized &&
        validationSystem.IsInitialized &&
        hitDetectionSystem.IsInitialized;

    public void Initialize(
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackValidationSystem validationSystem,
        EnemyMeleeHitDetectionSystem hitDetectionSystem)
    {
        this.attackSystem = null;
        this.targetSystem = null;
        this.damageSystem = null;
        this.validationSystem = null;
        this.hitDetectionSystem = null;

        if (attackSystem == null ||
            targetSystem == null ||
            damageSystem == null ||
            validationSystem == null ||
            hitDetectionSystem == null)
        {
            Debug.LogError(
                "EnemyMeleeAttackBehaviour requires all attack systems.");

            return;
        }

        if (!attackSystem.IsInitialized ||
            !targetSystem.IsInitialized ||
            !damageSystem.IsInitialized ||
            !validationSystem.IsInitialized ||
            !hitDetectionSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyMeleeAttackBehaviour received an uninitialized system.");

            return;
        }

        this.attackSystem = attackSystem;
        this.targetSystem = targetSystem;
        this.damageSystem = damageSystem;
        this.validationSystem = validationSystem;
        this.hitDetectionSystem = hitDetectionSystem;
    }

    public bool TryAttack()
    {
        if (!IsInitialized ||
            !validationSystem.CanAttack())
        {
            return false;
        }

        List<IDamageable> detectedTargets =
            hitDetectionSystem.DetectTargets();

        if (detectedTargets == null || detectedTargets.Count == 0)
            return false;

        Vector2 attackPosition =
            hitDetectionSystem.AttackOriginPosition;

        bool damageApplied = false;

        foreach (IDamageable target in detectedTargets)
        {
            if (target == null)
                continue;

            if (damageSystem.ApplyDamage(target, attackPosition))
                damageApplied = true;
        }

        return damageApplied;
    }
}
