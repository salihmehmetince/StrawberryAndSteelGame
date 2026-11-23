
using UnityEngine;

public class EnemyDetectionSettings
{
    public float DetectionRange { get; }
    public float VisionAngle { get; }

    public LayerMask TargetLayers { get; }
    public LayerMask ObstacleLayers { get; }

    public EnemyDetectionSettings(
        float detectionRange,
        float visionAngle,
        LayerMask targetLayers,
        LayerMask obstacleLayers)
    {
        DetectionRange = IsFinite(detectionRange)
            ? Mathf.Max(0f, detectionRange)
            : 0f;
        VisionAngle = IsFinite(visionAngle)
            ? Mathf.Clamp(visionAngle, 0f, 360f)
            : 0f;
        TargetLayers = targetLayers;
        ObstacleLayers = obstacleLayers;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
