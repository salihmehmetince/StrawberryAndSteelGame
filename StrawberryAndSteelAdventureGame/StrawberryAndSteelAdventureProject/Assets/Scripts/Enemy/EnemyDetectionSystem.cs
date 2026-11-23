
using UnityEngine;

public class EnemyDetectionSystem
{
    private Transform detectionOrigin;
    private Transform visualRoot;
    private EnemyDetectionSettings settings;

    public bool IsInitialized { get; private set; }

    public void Initialize(
        Transform detectionOrigin,
        Transform visualRoot,
        EnemyDetectionSettings settings)
    {
        this.detectionOrigin = null;
        this.visualRoot = null;
        this.settings = null;
        IsInitialized = false;

        if (detectionOrigin == null)
        {
            Debug.LogError(
                "EnemyDetectionSystem requires a Detection Origin.");
            return;
        }

        if (visualRoot == null)
        {
            Debug.LogError(
                "EnemyDetectionSystem requires a Visual Root.");
            return;
        }

        if (settings == null)
        {
            Debug.LogError(
                "EnemyDetectionSystem requires detection settings.");
            return;
        }

        if (float.IsNaN(settings.DetectionRange) || float.IsInfinity(settings.DetectionRange) ||
            float.IsNaN(settings.VisionAngle) || float.IsInfinity(settings.VisionAngle))
        {
            Debug.LogError("EnemyDetectionSystem received non-finite settings.");
            return;
        }

        this.detectionOrigin = detectionOrigin;
        this.visualRoot = visualRoot;
        this.settings = settings;

        IsInitialized = true;
    }

    public bool IsTargetLayerValid(Transform target)
    {
        if (!IsInitialized || target == null)
            return false;

        int targetLayerMask = 1 << target.gameObject.layer;

        return (settings.TargetLayers.value & targetLayerMask) != 0;
    }

    public bool IsTargetInRange(Transform target)
    {
        if (!IsInitialized || target == null)
            return false;

        float distance = Vector2.Distance(
            detectionOrigin.position,
            target.position);

        return distance <= settings.DetectionRange;
    }

    public bool IsTargetInVisionAngle(Transform target)
    {
        if (!IsInitialized || target == null)
            return false;

        Vector2 directionToTarget =
            (Vector2)(target.position - detectionOrigin.position);

        if (directionToTarget.sqrMagnitude <= 0.0001f)
            return true;

        float facingSign = Mathf.Sign(visualRoot.localScale.x);

        Vector2 facingDirection = Vector2.right * facingSign;

        float angle = Vector2.Angle(
            facingDirection,
            directionToTarget);

        return angle <= settings.VisionAngle * 0.5f;
    }

    public bool HasLineOfSight(Transform target)
    {
        if (!IsInitialized || target == null)
            return false;

        Vector2 origin = detectionOrigin.position;
        Vector2 direction = (Vector2)target.position - origin;

        float distance = direction.magnitude;

        if (distance <= 0.0001f)
            return true;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            origin,
            direction.normalized,
            distance,
            settings.ObstacleLayers);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            Transform hitRoot = hit.collider.transform.root;

            if (hitRoot == detectionOrigin.root || hitRoot == target.root)
                continue;

            return false;
        }

        return true;
    }

    public bool CanSeeTarget(Transform target)
    {
        return IsTargetLayerValid(target) &&
               IsTargetInRange(target) &&
               IsTargetInVisionAngle(target) &&
               HasLineOfSight(target);
    }
}
