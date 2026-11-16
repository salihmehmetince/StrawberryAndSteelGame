
using UnityEngine;

public class KnockbackBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;

    private float knockbackDuration;
    private float remainingDuration;

    private Vector2 knockbackVelocity;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Knockback;

    public bool IsActive { get; private set; }

    public void Initialize(
        EnemyController controller,
        float knockbackDuration)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        this.knockbackDuration = knockbackDuration;

        remainingDuration = 0f;
        knockbackVelocity = Vector2.zero;
        IsActive = false;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "KnockbackBehaviour requires a Rigidbody2D component.");

            return;
        }
    }

    public bool ApplyKnockback(Vector2 direction, float force)
    {
        if (enemyRigidbody == null || IsActive)
            return false;

        if (direction.sqrMagnitude == 0f || knockbackDuration <= 0f)
            return false;

        knockbackVelocity = direction.normalized * force;
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
