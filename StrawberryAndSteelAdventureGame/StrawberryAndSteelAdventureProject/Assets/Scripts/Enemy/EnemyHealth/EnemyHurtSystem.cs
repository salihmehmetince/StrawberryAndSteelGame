using UnityEngine;

public class EnemyHurtSystem
{
    private EnemyController enemyController;
    private Rigidbody2D enemyRigidbody;
    private KnockbackBehaviour knockbackBehaviour;

    private float knockbackForce;
    private float verticalKnockbackForce;
    private float fallbackDuration;
    private float remainingFallbackDuration;

    public bool IsInitialized =>
        enemyController != null &&
        enemyRigidbody != null &&
        knockbackBehaviour != null &&
        knockbackBehaviour.IsInitialized;

    public bool IsHurting { get; private set; }

    public void Initialize(
        EnemyController enemyController,
        Rigidbody2D enemyRigidbody,
        KnockbackBehaviour knockbackBehaviour,
        float knockbackForce,
        float verticalKnockbackForce,
        float fallbackDuration)
    {
        Clear();

        if (enemyController == null || enemyRigidbody == null ||
            knockbackBehaviour == null || !knockbackBehaviour.IsInitialized)
        {
            Debug.LogError(
                "EnemyHurtSystem requires an EnemyController, Rigidbody2D, and initialized KnockbackBehaviour.");
            return;
        }

        if (!IsValidForce(knockbackForce) ||
            !IsValidForce(verticalKnockbackForce) ||
            float.IsNaN(fallbackDuration) ||
            float.IsInfinity(fallbackDuration) ||
            fallbackDuration <= 0f)
        {
            Debug.LogError(
                "EnemyHurtSystem received invalid knockback or fallback duration values.");
            return;
        }

        this.enemyController = enemyController;
        this.enemyRigidbody = enemyRigidbody;
        this.knockbackBehaviour = knockbackBehaviour;
        this.knockbackForce = knockbackForce;
        this.verticalKnockbackForce = verticalKnockbackForce;
        this.fallbackDuration = fallbackDuration;
    }

    public void ApplyKnockback(Vector2 attackPosition)
    {
        if (!IsInitialized || IsHurting || !IsValidPosition(attackPosition))
            return;

        float directionX = enemyRigidbody.position.x - attackPosition.x;
        directionX = directionX < 0f ? -1f : 1f;

        Vector2 direction = new Vector2(
            directionX,
            verticalKnockbackForce > 0f ? 1f : 0f).normalized;

        Vector2 velocity = new Vector2(
            direction.x * knockbackForce,
            direction.y * verticalKnockbackForce);

        enemyController.TryActivateMovementAction(
            knockbackBehaviour,
            () => knockbackBehaviour.StartWithVelocity(velocity));

        IsHurting = true;
        remainingFallbackDuration = fallbackDuration;
    }

    /// <summary>Returns true when the animation-event fallback should complete the hurt.</summary>
    public bool Update(float deltaTime)
    {
        if (!IsHurting ||
            float.IsNaN(deltaTime) ||
            float.IsInfinity(deltaTime) ||
            deltaTime < 0f)
        {
            return false;
        }

        remainingFallbackDuration -= deltaTime;
        return remainingFallbackDuration <= 0f;
    }

    public void CompleteHurt()
    {
        IsHurting = false;
        remainingFallbackDuration = 0f;
    }

    private void Clear()
    {
        enemyController = null;
        enemyRigidbody = null;
        knockbackBehaviour = null;

        knockbackForce = 0f;
        verticalKnockbackForce = 0f;
        fallbackDuration = 0f;
        remainingFallbackDuration = 0f;
        IsHurting = false;
    }

    private static bool IsValidForce(float value)
    {
        return !float.IsNaN(value) &&
               !float.IsInfinity(value) &&
               value >= 0f;
    }

    private static bool IsValidPosition(Vector2 position)
    {
        return !float.IsNaN(position.x) &&
               !float.IsInfinity(position.x) &&
               !float.IsNaN(position.y) &&
               !float.IsInfinity(position.y);
    }
}
