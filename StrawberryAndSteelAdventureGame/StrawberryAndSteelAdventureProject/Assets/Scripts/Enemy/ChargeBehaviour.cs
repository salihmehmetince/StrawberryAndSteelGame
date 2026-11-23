
using UnityEngine;

public class ChargeBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;

    private float chargeSpeed;
    private float chargeDuration;
    private float cooldownDuration;
    private float cooldownRemaining;
    private float remainingDuration;

    private Vector2 chargeDirection;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Charge;

    public bool IsActive { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsReady => !IsActive && cooldownRemaining <= 0f;

    public void Initialize(
        EnemyController controller,
        float chargeSpeed,
        float chargeDuration,
        float cooldownDuration)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        this.chargeSpeed = chargeSpeed;
        this.chargeDuration = chargeDuration;
        this.cooldownDuration = cooldownDuration;

        remainingDuration = 0f;
        cooldownRemaining = 0f;
        IsActive = false;
        IsInitialized = false;

        if (enemyRigidbody == null ||
            !IsFinite(chargeSpeed) || chargeSpeed <= 0f ||
            !IsFinite(chargeDuration) || chargeDuration <= 0f ||
            !IsFinite(cooldownDuration) || cooldownDuration < 0f)
        {
            Debug.LogError(
                "ChargeBehaviour requires a Rigidbody2D and valid speed, duration, and cooldown values.");

            return;
        }

        IsInitialized = true;
    }

    public bool StartCharge(Vector2 targetPosition)
    {
        if (!IsInitialized || enemyRigidbody == null || !IsReady)
            return false;

        Vector2 direction =
            targetPosition - enemyRigidbody.position;

        if (!IsFinite(direction.x) || !IsFinite(direction.y) ||
            direction.sqrMagnitude <= 0f || chargeDuration <= 0f)
            return false;

        chargeDirection = direction.normalized;
        remainingDuration = chargeDuration;
        cooldownRemaining = cooldownDuration;
        IsActive = true;

        ApplyChargeVelocity();

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized || !IsActive || enemyRigidbody == null ||
            !IsFinite(deltaTime) || deltaTime < 0f)
            return;

        remainingDuration -= deltaTime;

        if (remainingDuration <= 0f)
        {
            Stop();
            return;
        }

        ApplyChargeVelocity();
    }

    public void TickCooldown(float deltaTime)
    {
        if (!IsInitialized || !IsFinite(deltaTime) || deltaTime < 0f)
            return;

        cooldownRemaining = Mathf.Max(0f, cooldownRemaining - deltaTime);
    }

    private void ApplyChargeVelocity()
    {
        enemyRigidbody.linearVelocity =
            chargeDirection * chargeSpeed;
    }

    public void Stop()
    {
        IsActive = false;
        remainingDuration = 0f;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
