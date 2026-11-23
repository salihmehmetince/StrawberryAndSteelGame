using UnityEngine;

[System.Serializable]
public sealed class EnemyChargeSettings
{
    [SerializeField] private bool enabled;
    [SerializeField, Min(0f)] private float minimumTriggerDistance = 2f;
    [Tooltip("Set to zero to remove the upper distance limit.")]
    [SerializeField, Min(0f)] private float maximumTriggerDistance = 7f;
    [SerializeField, Min(0.01f)] private float speed = 9f;
    [SerializeField, Min(0.01f)] private float duration = 0.6f;
    [SerializeField, Min(0f)] private float cooldown = 2f;

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
        maximumTriggerDistance = ClampFinite(maximumTriggerDistance, 7f, 0f);
        speed = ClampFinite(speed, 9f, 0.01f);
        duration = ClampFinite(duration, 0.6f, 0.01f);
        cooldown = ClampFinite(cooldown, 2f, 0f);

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
