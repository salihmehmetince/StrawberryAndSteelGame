
using UnityEngine;

public class EnemyAttackTargetSystem
{
    private Transform attackOrigin;
    private Transform currentTarget;
    private float attackRange;

    public bool IsInitialized =>
        attackOrigin != null &&
        !float.IsNaN(attackRange) &&
        !float.IsInfinity(attackRange) &&
        attackRange >= 0f;

    public Transform CurrentTarget =>
        HasValidTarget() ? currentTarget : null;

    public bool HasTarget => HasValidTarget();

    public bool IsTargetInRange
    {
        get
        {
            if (!IsInitialized || !HasValidTarget())
                return false;

            return Vector2.Distance(
                attackOrigin.position,
                currentTarget.position) <= attackRange;
        }
    }

    public void Initialize(
        Transform attackOrigin,
        float attackRange)
    {
        this.attackOrigin = null;
        this.currentTarget = null;
        this.attackRange = 0f;

        if (attackOrigin == null ||
            float.IsNaN(attackRange) ||
            float.IsInfinity(attackRange) ||
            attackRange < 0f)
        {
            Debug.LogError(
                "EnemyAttackTargetSystem received invalid initialization values.");

            return;
        }

        this.attackOrigin = attackOrigin;
        this.attackRange = attackRange;
    }

    public void SetTarget(Transform target)
    {
        if (target == null)
        {
            ClearTarget();
            return;
        }

        if (!target.gameObject.activeInHierarchy ||
            !target.root.gameObject.activeInHierarchy)
        {
            ClearTarget();
            return;
        }

        currentTarget = target;
    }

    public bool HasValidTarget()
    {
        return currentTarget != null &&
               currentTarget.gameObject.activeInHierarchy &&
               currentTarget.root.gameObject.activeInHierarchy;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    public bool IsTargetInRangeOf(float range)
    {
        if (!IsInitialized ||
            !HasValidTarget() ||
            float.IsNaN(range) ||
            float.IsInfinity(range) ||
            range < 0f)
        {
            return false;
        }

        return Vector2.Distance(
            attackOrigin.position,
            currentTarget.position) <= range;
    }
}
