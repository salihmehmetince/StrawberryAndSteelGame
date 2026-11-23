
using UnityEngine;

public class KnockbackBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;

    private float knockbackDuration;
    private float remainingDuration;

    private Vector2 knockbackVelocity;
    private bool isInitialized;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Knockback;

    public bool IsActive { get; private set; }

    public bool IsInitialized => isInitialized;

    public void Initialize(
        EnemyController controller,
        float knockbackDuration)
    {
        enemyRigidbody = controller != null
            ? controller.EnemyRigidbody
            : null;

        this.knockbackDuration = knockbackDuration;

        remainingDuration = 0f;
        knockbackVelocity = Vector2.zero;
        IsActive = false;
        isInitialized = false;

        if (enemyRigidbody == null ||
            float.IsNaN(knockbackDuration) ||
            float.IsInfinity(knockbackDuration) ||
            knockbackDuration <= 0f)
        {
            Debug.LogError(
                "KnockbackBehaviour requires a Rigidbody2D and a positive finite duration.");

            return;
        }

        isInitialized = true;
    }

    public bool ApplyKnockback(Vector2 direction, float force)
    {
        if (!IsInitialized || IsActive)
            return false;

        if (direction.sqrMagnitude == 0f ||
            float.IsNaN(force) ||
            float.IsInfinity(force) ||
            force <= 0f)
            return false;

        return StartWithVelocity(direction.normalized * force);
    }

    public bool StartWithVelocity(Vector2 velocity)
    {
        if (!IsInitialized || IsActive ||
            float.IsNaN(velocity.x) || float.IsInfinity(velocity.x) ||
            float.IsNaN(velocity.y) || float.IsInfinity(velocity.y) ||
            velocity.sqrMagnitude <= 0f)
        {
            return false;
        }

        knockbackVelocity = velocity;
        remainingDuration = knockbackDuration;
        IsActive = true;

        ApplyKnockbackVelocity();

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

        ApplyKnockbackVelocity();
    }

    private void ApplyKnockbackVelocity()
    {
        enemyRigidbody.linearVelocity = knockbackVelocity;
    }

    public void Stop()
    {
        IsActive = false;
        remainingDuration = 0f;
        knockbackVelocity = Vector2.zero;
    }
}
