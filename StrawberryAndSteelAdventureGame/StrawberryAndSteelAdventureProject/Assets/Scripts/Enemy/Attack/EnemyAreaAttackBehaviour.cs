
using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaAttackBehaviour : IEnemyAttackBehaviour
{
    private EnemyAttackSystem attackSystem;
    private EnemyAttackDamageSystem damageSystem;
    private EnemyAttackTargetSystem targetSystem;
    private EnemyAttackValidationSystem validationSystem;

    private Transform attackOrigin;
    private float attackRadius;
    private LayerMask targetLayers;

    public bool IsInitialized =>
        attackSystem != null &&
        damageSystem != null &&
        targetSystem != null &&
        validationSystem != null &&
        attackOrigin != null &&
        attackSystem.IsInitialized &&
        damageSystem.IsInitialized &&
        targetSystem.IsInitialized &&
        validationSystem.IsInitialized;

    public void Initialize(
        EnemyAttackSystem attackSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackValidationSystem validationSystem,
        Transform attackOrigin,
        float attackRadius,
        LayerMask targetLayers)
    {
        this.attackSystem = null;
        this.damageSystem = null;
        this.targetSystem = null;
        this.validationSystem = null;

        this.attackOrigin = null;
        this.attackRadius = 0f;
        this.targetLayers = default;

        if (attackSystem == null ||
            damageSystem == null ||
            targetSystem == null ||
            validationSystem == null ||
            attackOrigin == null ||
            float.IsNaN(attackRadius) ||
            float.IsInfinity(attackRadius) ||
            attackRadius <= 0f)
        {
            Debug.LogError(
                "EnemyAreaAttackBehaviour received invalid initialization values.");

            return;
        }

        if (!attackSystem.IsInitialized ||
            !damageSystem.IsInitialized ||
            !targetSystem.IsInitialized ||
            !validationSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAreaAttackBehaviour received an uninitialized system.");

            return;
        }

        this.attackSystem = attackSystem;
        this.damageSystem = damageSystem;
        this.targetSystem = targetSystem;
        this.validationSystem = validationSystem;

        this.attackOrigin = attackOrigin;
        this.attackRadius = attackRadius;
        this.targetLayers = targetLayers;
    }

    public bool TryAttack()
    {
        if (!IsInitialized ||
            !validationSystem.CanAttack())
        {
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            attackRadius,
            targetLayers);

        HashSet<IDamageable> uniqueTargets =
            new HashSet<IDamageable>();

        foreach (Collider2D targetCollider in colliders)
        {
            if (targetCollider == null)
                continue;

            if (targetCollider.transform.root == attackOrigin.root)
                continue;

            MonoBehaviour[] components =
                targetCollider.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                if (component is IDamageable damageable)
                {
                    uniqueTargets.Add(damageable);
                    break;
                }
            }
        }

        if (uniqueTargets.Count == 0)
            return false;

        Vector2 attackPosition = attackOrigin.position;

        bool damageApplied = false;

        foreach (IDamageable target in uniqueTargets)
        {
            if (target == null)
                continue;

            if (damageSystem.ApplyDamage(target, attackPosition))
                damageApplied = true;
        }

        return damageApplied;
    }
}
