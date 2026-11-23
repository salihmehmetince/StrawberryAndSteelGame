using UnityEngine;

[System.Serializable]
public sealed class EnemyDashSettings
{
    [SerializeField] private bool enabled;
    [SerializeField, Min(0f)] private float minimumTriggerDistance = 2f;
    [Tooltip("Set to zero to remove the upper distance limit.")]
    [SerializeField, Min(0f)] private float maximumTriggerDistance = 5f;
    [SerializeField, Min(0.01f)] private float speed = 12f;
    [SerializeField, Min(0.01f)] private float duration = 0.2f;
    [SerializeField, Min(0f)] private float cooldown = 1f;

    public bool Enabled => enabled;
    public float MinimumTriggerDistance => minimumTriggerDistance;
    public float MaximumTriggerDistance => maximumTriggerDistance;
    public float Speed => speed;
    public float Duration => duration;
    public float Cooldown => cooldown;

    public bool ShouldTrigger(float distance)
    {
        return enabled && IsFinite(distance) && distance >= minimumTriggerDistance &&
               (maximumTriggerDistance <= 0f || distance <= maximumTriggerDistance);
    }

    public void Validate()
    {
        minimumTriggerDistance = ClampFinite(minimumTriggerDistance, 2f, 0f);
        maximumTriggerDistance = ClampFinite(maximumTriggerDistance, 5f, 0f);
        speed = ClampFinite(speed, 12f, 0.01f);
        duration = ClampFinite(duration, 0.2f, 0.01f);
        cooldown = ClampFinite(cooldown, 1f, 0f);

        if (maximumTriggerDistance > 0f && maximumTriggerDistance < minimumTriggerDistance)
            maximumTriggerDistance = minimumTriggerDistance;
    }

    private static float ClampFinite(float value, float fallback, float minimum)
    {
        if (!IsFinite(value))
            return fallback;

        return Mathf.Max(minimum, value);
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
