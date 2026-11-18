
using UnityEngine;

public class EnemyDeathSystem
{
    private EnemyController enemyController;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;
    private Animator enemyAnimator;

    private float destroyDelay;
    private float destroyTimer;

    public bool IsInitialized => enemyController != null;
    public bool IsDead { get; private set; }
    public bool IsDeathAnimationCompleted { get; private set; }
    public bool IsDestroyed { get; private set; }

    public void Initialize(
        EnemyController enemyController,
        Rigidbody2D enemyRigidbody,
        Collider2D enemyCollider,
        Animator enemyAnimator,
        float destroyDelay)
    {
        if (enemyController == null)
        {
            Debug.LogError(
                "EnemyDeathSystem requires an EnemyController.");

            return;
        }

        if (destroyDelay < 0f)
        {
            Debug.LogError(
                "EnemyDeathSystem received an invalid destroy delay.");

            return;
        }

        this.enemyController = enemyController;
        this.enemyRigidbody = enemyRigidbody;
        this.enemyCollider = enemyCollider;
        this.enemyAnimator = enemyAnimator;
        this.destroyDelay = destroyDelay;

        IsDead = false;
        IsDeathAnimationCompleted = false;
        IsDestroyed = false;
        destroyTimer = 0f;
    }

    public void Die()
    {
        if (!IsInitialized || IsDead)
            return;

        IsDead = true;

        enemyController.StopNormalMovement();

        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
            enemyRigidbody.simulated = false;
        }

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        if (enemyAnimator != null)
            enemyAnimator.SetTrigger("Die");
        else
            CompleteDeathAnimation();
    }

    public void CompleteDeathAnimation()
    {
        if (!IsDead || IsDeathAnimationCompleted)
            return;

        IsDeathAnimationCompleted = true;
        destroyTimer = destroyDelay;
    }

    public void Update(float deltaTime)
    {
        if (!IsDead ||
            !IsDeathAnimationCompleted ||
            IsDestroyed)
            return;

        destroyTimer -= deltaTime;

        if (destroyTimer <= 0f)
            DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        if (IsDestroyed)
            return;

        IsDestroyed = true;
        Object.Destroy(enemyController.gameObject);
    }
}
