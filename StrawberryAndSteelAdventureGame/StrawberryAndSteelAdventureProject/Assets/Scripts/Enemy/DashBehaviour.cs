
using UnityEngine;

public class DashBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;

    private float dashSpeed;
    private float dashDuration;
    private float remainingDuration;

    private Vector2 dashDirection;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Dash;

    public bool IsActive { get; private set; }

    public void Initialize(
        EnemyController controller,
        float dashSpeed,
        float dashDuration)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        this.dashSpeed = dashSpeed;
        this.dashDuration = dashDuration;

        remainingDuration = 0f;
        IsActive = false;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "DashBehaviour requires a Rigidbody2D component.");

            return;
        }
    }

    public bool StartDash(Vector2 direction)
    {
        if (enemyRigidbody == null || IsActive)
            return false;

        if (direction.sqrMagnitude == 0f || dashDuration <= 0f)
            return false;

        dashDirection = direction.normalized;
        remainingDuration = dashDuration;
        IsActive = true;

        ApplyDashVelocity();

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsActive || enemyRigidbody == null)
            return;

        remainingDuration -= deltaTime;

        if (remainingDuration <= 0f)
        {
            Stop();
            return;
        }

        ApplyDashVelocity();
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
}
