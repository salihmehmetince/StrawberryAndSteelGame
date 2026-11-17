
using UnityEngine;

public class EnemyTargetScanner
{
    private EnemyDetectionSystem detectionSystem;
    private Transform detectionOrigin;

    private float scanRadius;
    private LayerMask targetLayers;

    public bool IsInitialized { get; private set; }

    public void Initialize(
        EnemyDetectionSystem detectionSystem,
        Transform detectionOrigin,
        float scanRadius,
        LayerMask targetLayers)
    {
        if (detectionSystem == null)
        {
            Debug.LogError(
                "EnemyTargetScanner requires an EnemyDetectionSystem.");
            return;
        }

        if (detectionOrigin == null)
        {
            Debug.LogError(
                "EnemyTargetScanner requires a Detection Origin.");
            return;
        }

        if (scanRadius <= 0f)
        {
            Debug.LogError(
                "EnemyTargetScanner requires a positive scan radius.");
            return;
        }

        this.detectionSystem = detectionSystem;
        this.detectionOrigin = detectionOrigin;
        this.scanRadius = scanRadius;
        this.targetLayers = targetLayers;

        IsInitialized = true;
    }


    public Transform FindNearestVisibleTarget()
    {
        if (!IsInitialized)
            return null;

        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(
            detectionOrigin.position,
            scanRadius,
            targetLayers);

        Transform nearestTarget = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (Collider2D detectedCollider in detectedColliders)
        {
            if (detectedCollider == null)
                continue;

            Transform candidate = detectedCollider.transform;

            if (candidate.root == detectionOrigin.root)
                continue;

            if (!detectionSystem.CanSeeTarget(candidate))
                continue;

            float distance = Vector2.Distance(
                detectionOrigin.position,
                candidate.position);

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestTarget = candidate;
        }

        return nearestTarget;
    }
}
