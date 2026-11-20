
using UnityEngine;

public class EnemyAttackCooldownSystem
{
    private float cooldownDuration;
    private float remainingCooldown;

    public bool IsInitialized { get; private set; }

    public bool IsOnCooldown =>
        IsInitialized && remainingCooldown > 0f;

    public float RemainingCooldown => remainingCooldown;

    public float CooldownPercentage =>
        cooldownDuration > 0f
            ? remainingCooldown / cooldownDuration
            : 0f;

    public void Initialize(float cooldownDuration)
    {
        this.cooldownDuration = 0f;
        remainingCooldown = 0f;
        IsInitialized = false;

        if (float.IsNaN(cooldownDuration) ||
            float.IsInfinity(cooldownDuration) ||
            cooldownDuration < 0f)
        {
            Debug.LogError(
                "Enemy attack cooldown duration must be a finite, non-negative value.");

            return;
        }

        this.cooldownDuration = cooldownDuration;
        IsInitialized = true;
    }
    public bool TryStartCooldown()
    {
        if (!IsInitialized || IsOnCooldown)
            return false;

        remainingCooldown = cooldownDuration;

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized ||
            !IsOnCooldown ||
            float.IsNaN(deltaTime) ||
            float.IsInfinity(deltaTime) ||
            deltaTime < 0f)
        {
            return;
        }

        remainingCooldown = Mathf.Max(
            0f,
            remainingCooldown - deltaTime);
    }
    public void ResetCooldown()
    {
        if (!IsInitialized)
            return;

        remainingCooldown = 0f;
    }
}
