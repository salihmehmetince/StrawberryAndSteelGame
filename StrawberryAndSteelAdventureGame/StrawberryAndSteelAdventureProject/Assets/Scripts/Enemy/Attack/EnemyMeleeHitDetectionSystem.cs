
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeHitDetectionSystem
{
    private Transform attackOrigin;
    private float hitRadius;
    private LayerMask targetLayers;

    public bool IsInitialized => attackOrigin != null;

    public Vector2 AttackOriginPosition =>
    attackOrigin != null
        ? (Vector2)attackOrigin.position
        : Vector2.zero;
    public void Initialize(
        Transform attackOrigin,
        float hitRadius,
        LayerMask targetLayers)
    {
        if (attackOrigin == null || hitRadius < 0f)
        {
            Debug.LogError(
                "EnemyMeleeHitDetectionSystem received invalid initialization values.");
            return;
        }

        this.attackOrigin = attackOrigin;
        this.hitRadius = hitRadius;
        this.targetLayers = targetLayers;
    }

    public List<IDamageable> DetectTargets()
    {
        List<IDamageable> detectedTargets = new List<IDamageable>();

        if (!IsInitialized)
            return detectedTargets;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            hitRadius,
            targetLayers);

        HashSet<IDamageable> uniqueTargets =
            new HashSet<IDamageable>();

        foreach (Collider2D targetCollider in colliders)
        {
            if (targetCollider.transform.root == attackOrigin.root)
                continue;

            MonoBehaviour[] components =
                targetCollider.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                if (component is IDamageable damageable)
                {
                    if (uniqueTargets.Add(damageable))
                        detectedTargets.Add(damageable);

                    break;
                }
            }
        }

        return detectedTargets;
    }
}
