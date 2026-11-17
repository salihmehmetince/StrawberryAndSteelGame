
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

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction.normalized,
            distance,
            settings.ObstacleLayers);

        return hit.collider == null;
    }

    public bool CanSeeTarget(Transform target)
    {
        return IsTargetLayerValid(target) &&
               IsTargetInRange(target) &&
               IsTargetInVisionAngle(target) &&
               HasLineOfSight(target);
    }
}
