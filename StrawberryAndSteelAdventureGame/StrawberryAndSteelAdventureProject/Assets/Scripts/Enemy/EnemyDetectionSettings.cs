
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
        DetectionRange = Mathf.Max(0f, detectionRange);
        VisionAngle = Mathf.Clamp(visionAngle, 0f, 360f);
        TargetLayers = targetLayers;
        ObstacleLayers = obstacleLayers;
    }
}
