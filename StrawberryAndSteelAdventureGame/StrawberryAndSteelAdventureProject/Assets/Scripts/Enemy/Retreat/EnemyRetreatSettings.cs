using UnityEngine;

[System.Serializable]
public sealed class EnemyRetreatSettings
{
    [SerializeField] private bool enabled;
    [Tooltip("Retreat when the target is closer than this distance. Set to zero to disable this trigger.")]
    [SerializeField] private float triggerDistance = 1.25f;
    [Tooltip("Retreat when health is at or below this percentage. Set to zero to disable this trigger.")]
    [Range(0f, 100f)]
    [SerializeField] private float healthThresholdPercent = 25f;
    [SerializeField] private float desiredDistance = 3f;
    [SerializeField] private float speedMultiplier = 1.25f;
    [SerializeField] private float maximumDuration = 1.5f;
    [SerializeField] private float cooldown = 2f;

    public bool Enabled => enabled;
    public float TriggerDistance => triggerDistance;
    public float HealthThresholdPercent => healthThresholdPercent;
    public float DesiredDistance => desiredDistance;
    public float SpeedMultiplier => speedMultiplier;
    public float MaximumDuration => maximumDuration;
    public float Cooldown => cooldown;

    public EnemyRetreatSettings CreateValidatedCopy()
    {
        EnemyRetreatSettings copy = new EnemyRetreatSettings
        {
            enabled = enabled,
            triggerDistance = triggerDistance,
            healthThresholdPercent = healthThresholdPercent,
            desiredDistance = desiredDistance,
            speedMultiplier = speedMultiplier,
            maximumDuration = maximumDuration,
            cooldown = cooldown
        };

        copy.Validate();
        return copy;
    }

    public void Validate()
    {
        triggerDistance = ClampFinite(triggerDistance, 1.25f, 0f);
        healthThresholdPercent = ClampFinite(healthThresholdPercent, 25f, 0f, 100f);
        desiredDistance = ClampFinite(desiredDistance, 3f, 0.01f);
        speedMultiplier = ClampFinite(speedMultiplier, 1.25f, 0.01f);
        maximumDuration = ClampFinite(maximumDuration, 1.5f, 0.01f);
        cooldown = ClampFinite(cooldown, 2f, 0f);
    }

    private static float ClampFinite(float value, float fallback, float minimum, float maximum = float.PositiveInfinity)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            value = fallback;

        return Mathf.Clamp(value, minimum, maximum);
    }
}
