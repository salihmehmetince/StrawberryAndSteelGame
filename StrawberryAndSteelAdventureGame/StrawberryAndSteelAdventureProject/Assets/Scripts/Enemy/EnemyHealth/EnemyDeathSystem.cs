
using UnityEngine;

public class EnemyDeathSystem
{
    private EnemyController enemyController;
    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;

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
        float destroyDelay)
    {
        Clear();

        if (enemyController == null)
        {
            Debug.LogError(
                "EnemyDeathSystem requires an EnemyController.");

            return;
        }

        this.enemyController = enemyController;
        this.enemyRigidbody = enemyRigidbody;
        this.enemyCollider = enemyCollider;

        this.destroyDelay = Mathf.Max(0f, destroyDelay);
    }

    public void Die()
    {
        if (!IsInitialized || IsDead)
            return;

        IsDead = true;

        enemyController.StopMovement();

        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
            enemyRigidbody.simulated = false;
        }

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        if (enemyController.HasAnimator)
        {
            enemyController.PlayDeathAnimation();
        }
        else
        {
            CompleteDeathAnimation();
        }
    }

    public void CompleteDeathAnimation()
    {
        if (!IsInitialized ||
            !IsDead ||
            IsDeathAnimationCompleted)
        {
            return;
        }

        IsDeathAnimationCompleted = true;
        destroyTimer = destroyDelay;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized ||
            !IsDeathAnimationCompleted ||
            IsDestroyed)
        {
            return;
        }

        destroyTimer -= deltaTime;

        if (destroyTimer > 0f)
            return;

        IsDestroyed = true;

        Object.Destroy(enemyController.gameObject);
    }

    private void Clear()
    {
        enemyController = null;
        enemyRigidbody = null;
        enemyCollider = null;

        destroyDelay = 0f;
        destroyTimer = 0f;

        IsDead = false;
        IsDeathAnimationCompleted = false;
        IsDestroyed = false;
    }
}
