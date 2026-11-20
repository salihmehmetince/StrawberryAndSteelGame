
using UnityEngine;

public class EnemyAttackLineOfSightSystem
{
    private Transform attackOrigin;
    private LayerMask obstacleLayers;

    public bool IsInitialized =>
        attackOrigin != null;

    public void Initialize(
        Transform attackOrigin,
        LayerMask obstacleLayers)
    {
        this.attackOrigin = null;
        this.obstacleLayers = default;

        if (attackOrigin == null)
        {
            Debug.LogError(
                "EnemyAttackLineOfSightSystem requires an attack origin.");

            return;
        }

        this.attackOrigin = attackOrigin;
        this.obstacleLayers = obstacleLayers;
    }

    public bool HasLineOfSight(Transform target)
    {
        if (!IsInitialized ||
            target == null ||
            !target.gameObject.activeInHierarchy ||
            !target.root.gameObject.activeInHierarchy)
        {
            return false;
        }

        Vector2 origin = attackOrigin.position;
        Vector2 destination = target.position;

        Vector2 direction = destination - origin;
        float distance = direction.magnitude;

        if (distance <= 0f)
            return true;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction.normalized,
            distance,
            obstacleLayers);

        return hit.collider == null;
    }
}
