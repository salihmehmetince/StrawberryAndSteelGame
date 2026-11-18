
using UnityEngine;

public class EnemyAttackTargetSystem
{
    private Transform attackOrigin;
    private Transform currentTarget;
    private float attackRange;

    public bool IsInitialized =>
        attackOrigin != null &&
        attackRange >= 0f;

    public Transform CurrentTarget => currentTarget;

    public bool HasTarget => currentTarget != null;

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
        if (attackOrigin == null || attackRange < 0f)
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
        currentTarget = target;
    }


    public bool HasValidTarget()
    {
        if (currentTarget == null)
            return false;

        if (!currentTarget.gameObject.activeInHierarchy)
            return false;

        if (!currentTarget.root.gameObject.activeInHierarchy)
            return false;

        return true;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    public bool IsTargetInRangeOf(float range)
    {
        if (!IsInitialized || !HasValidTarget() || range < 0f)
            return false;

        return Vector2.Distance(
            attackOrigin.position,
            currentTarget.position) <= range;
    }
}
