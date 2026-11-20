
using UnityEngine;

public class EnemyHurtSystem
{
    private EnemyController enemyController;
    private Rigidbody2D enemyRigidbody;
    private Animator enemyAnimator;

    private float knockbackForce;
    private float verticalKnockbackForce;

    public bool IsInitialized =>
        enemyController != null &&
        enemyRigidbody != null;

    public bool IsHurting { get; private set; }

    public void Initialize(
        EnemyController enemyController,
        Rigidbody2D enemyRigidbody,
        Animator enemyAnimator,
        float knockbackForce,
        float verticalKnockbackForce)
    {
        Clear();

        if (enemyController == null || enemyRigidbody == null)
        {
            Debug.LogError(
                "EnemyHurtSystem requires an EnemyController and Rigidbody2D.");

            return;
        }

        if (!IsValidForce(knockbackForce) ||
            !IsValidForce(verticalKnockbackForce))
        {
            Debug.LogError(
                "EnemyHurtSystem received invalid knockback values.");

            return;
        }

        this.enemyController = enemyController;
        this.enemyRigidbody = enemyRigidbody;
        this.enemyAnimator = enemyAnimator;

        this.knockbackForce = knockbackForce;
        this.verticalKnockbackForce = verticalKnockbackForce;
    }

    public void ApplyKnockback(Vector2 attackPosition)
    {
        if (!IsInitialized || IsHurting)
            return;

        if (!IsValidPosition(attackPosition))
            return;

        float directionX =
            enemyRigidbody.position.x - attackPosition.x;

        directionX = directionX < 0f ? -1f : 1f;

        Vector2 direction = new Vector2(
            directionX,
            verticalKnockbackForce > 0f ? 1f : 0f).normalized;

        enemyRigidbody.linearVelocity = new Vector2(
            direction.x * knockbackForce,
            direction.y * verticalKnockbackForce);

        IsHurting = true;

        if (enemyAnimator != null)
            enemyAnimator.SetTrigger("Hurt");
    }

    public void CompleteHurt()
    {
        IsHurting = false;
    }

    private void Clear()
    {
        enemyController = null;
        enemyRigidbody = null;
        enemyAnimator = null;

        knockbackForce = 0f;
        verticalKnockbackForce = 0f;

        IsHurting = false;
    }

    private bool IsValidForce(float value)
    {
        return !float.IsNaN(value) &&
               !float.IsInfinity(value) &&
               value >= 0f;
    }

    private bool IsValidPosition(Vector2 position)
    {
        return !float.IsNaN(position.x) &&
               !float.IsInfinity(position.x) &&
               !float.IsNaN(position.y) &&
               !float.IsInfinity(position.y);
    }
}
