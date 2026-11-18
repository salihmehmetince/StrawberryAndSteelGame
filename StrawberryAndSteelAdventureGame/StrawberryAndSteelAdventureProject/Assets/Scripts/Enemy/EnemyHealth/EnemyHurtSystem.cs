
using UnityEngine;

public class EnemyHurtSystem
{
    private EnemyController enemyController;
    private Rigidbody2D enemyRigidbody;
    private Animator enemyAnimator;

    private float knockbackForce;
    private float verticalKnockbackForce;

    public bool IsInitialized => enemyController != null;
    public bool IsHurting { get; private set; }

    public void Initialize(
        EnemyController enemyController,
        Rigidbody2D enemyRigidbody,
        Animator enemyAnimator,
        float knockbackForce,
        float verticalKnockbackForce)
    {
        if (enemyController == null || enemyRigidbody == null)
        {
            Debug.LogError(
                "EnemyHurtSystem requires an EnemyController and Rigidbody2D.");

            return;
        }

        if (knockbackForce < 0f || verticalKnockbackForce < 0f)
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

        IsHurting = false;
    }

    public void ApplyKnockback(Vector2 attackPosition)
    {
        if (!IsInitialized || IsHurting)
            return;

        float directionX = Mathf.Sign(
            enemyRigidbody.position.x - attackPosition.x);

        if (directionX == 0f)
            directionX = 1f;

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
}
