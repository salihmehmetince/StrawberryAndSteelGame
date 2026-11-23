using UnityEngine;

[System.Serializable]
public sealed class EnemyPatrolSettings
{
    [SerializeField] private bool enabled;
    [Tooltip("Enemy-local offsets from its spawn position. Use at least two points.")]
    [SerializeField] private Vector2[] localWaypoints = new Vector2[0];
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float arrivalDistance = 0.15f;
    [SerializeField] private float waitAtWaypoint = 0.5f;
    [SerializeField] private bool pingPong = true;

    public bool Enabled => enabled;
    public Vector2[] LocalWaypoints => localWaypoints;
    public float SpeedMultiplier => speedMultiplier;
    public float ArrivalDistance => arrivalDistance;
    public float WaitAtWaypoint => waitAtWaypoint;
    public bool PingPong => pingPong;

    public EnemyPatrolSettings CreateValidatedCopy()
    {
        EnemyPatrolSettings copy = new EnemyPatrolSettings
        {
            enabled = enabled,
            localWaypoints = localWaypoints != null
                ? (Vector2[])localWaypoints.Clone()
                : new Vector2[0],
            speedMultiplier = speedMultiplier,
            arrivalDistance = arrivalDistance,
            waitAtWaypoint = waitAtWaypoint,
            pingPong = pingPong
        };

        copy.Validate();
        return copy;
    }

    public void Validate()
    {
        speedMultiplier = ClampFinite(speedMultiplier, 1f, 0.01f);
        arrivalDistance = ClampFinite(arrivalDistance, 0.15f, 0.01f);
        waitAtWaypoint = ClampFinite(waitAtWaypoint, 0.5f, 0f);

        if (localWaypoints == null)
            localWaypoints = new Vector2[0];

        for (int i = 0; i < localWaypoints.Length; i++)
        {
            Vector2 point = localWaypoints[i];
            if (float.IsNaN(point.x) || float.IsInfinity(point.x) ||
                float.IsNaN(point.y) || float.IsInfinity(point.y))
            {
                localWaypoints[i] = Vector2.zero;
            }
        }
    }

    private static float ClampFinite(float value, float fallback, float minimum)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            return fallback;

        return Mathf.Max(minimum, value);
    }
}
