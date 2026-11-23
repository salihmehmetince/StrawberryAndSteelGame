
using UnityEngine;

public class DashBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;

    private float dashSpeed;
    private float dashDuration;
    private float cooldownDuration;
    private float cooldownRemaining;
    private float remainingDuration;

    private Vector2 dashDirection;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Dash;

    public bool IsActive { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsReady => !IsActive && cooldownRemaining <= 0f;

    public void Initialize(
        EnemyController controller,
        float dashSpeed,
        float dashDuration,
        float cooldownDuration)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        this.dashSpeed = dashSpeed;
        this.dashDuration = dashDuration;
        this.cooldownDuration = cooldownDuration;

        remainingDuration = 0f;
        cooldownRemaining = 0f;
        IsActive = false;
        IsInitialized = false;

        if (enemyRigidbody == null ||
            !IsFinite(dashSpeed) || dashSpeed <= 0f ||
            !IsFinite(dashDuration) || dashDuration <= 0f ||
            !IsFinite(cooldownDuration) || cooldownDuration < 0f)
        {
            Debug.LogError(
                "DashBehaviour requires a Rigidbody2D and valid speed, duration, and cooldown values.");

            return;
        }

        IsInitialized = true;
    }

    public bool StartDash(Vector2 direction)
    {
        if (!IsInitialized || enemyRigidbody == null || !IsReady)
            return false;

        if (!IsFinite(direction.x) || !IsFinite(direction.y) ||
            direction.sqrMagnitude <= 0f || dashDuration <= 0f)
            return false;

        dashDirection = direction.normalized;
        remainingDuration = dashDuration;
        cooldownRemaining = cooldownDuration;
        IsActive = true;

        ApplyDashVelocity();

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

        ApplyDashVelocity();
    }

    public void TickCooldown(float deltaTime)
    {
        if (!IsInitialized || !IsFinite(deltaTime) || deltaTime < 0f)
            return;

        cooldownRemaining = Mathf.Max(0f, cooldownRemaining - deltaTime);
    }

    private void ApplyDashVelocity()
    {
        enemyRigidbody.linearVelocity =
            dashDirection * dashSpeed;
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
