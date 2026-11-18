
using UnityEngine;

public class EnemyAttackCooldownSystem
{
    private float cooldownDuration;
    private float remainingCooldown;

    public bool IsInitialized { get; private set; }

    public bool IsOnCooldown => remainingCooldown > 0f;

    public float RemainingCooldown => remainingCooldown;

    public float CooldownPercentage =>
        cooldownDuration > 0f
            ? remainingCooldown / cooldownDuration
            : 0f;

    public void Initialize(float cooldownDuration)
    {
        if (cooldownDuration < 0f)
        {
            Debug.LogError(
                "EnemyAttackCooldownSystem received an invalid cooldown duration.");
            return;
        }

        this.cooldownDuration = cooldownDuration;
        remainingCooldown = 0f;

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
        if (!IsInitialized || remainingCooldown <= 0f)
            return;

        remainingCooldown = Mathf.Max(
            0f,
            remainingCooldown - deltaTime);
    }

    public void ResetCooldown()
    {
        remainingCooldown = 0f;
    }
}
