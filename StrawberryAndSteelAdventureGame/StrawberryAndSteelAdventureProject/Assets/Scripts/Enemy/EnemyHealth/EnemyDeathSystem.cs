
using UnityEngine;

public class EnemyDeathSystem
{
    private EnemyController enemyController;
    private Rigidbody2D enemyRigidbody;
    private Collider2D[] enemyColliders;

    private float destroyDelay;
    private float destroyTimer;
    private float animationTimeout;
    private float remainingAnimationTimeout;

    public bool IsInitialized => enemyController != null;
    public bool IsDead { get; private set; }
    public bool IsDeathAnimationCompleted { get; private set; }
    public bool IsDestroyed { get; private set; }

    public void Initialize(
        EnemyController enemyController,
        Rigidbody2D enemyRigidbody,
        Collider2D[] enemyColliders,
        float destroyDelay,
        float animationTimeout)
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
        this.enemyColliders = enemyColliders;

        this.destroyDelay = IsValidDuration(destroyDelay)
            ? Mathf.Max(0f, destroyDelay)
            : 0f;
        this.animationTimeout = IsValidDuration(animationTimeout)
            ? Mathf.Max(0.01f, animationTimeout)
            : 5f;
    }

    public void Die()
    {
        if (!IsInitialized || IsDead)
            return;

        IsDead = true;
        remainingAnimationTimeout = animationTimeout;

        enemyController.OnDeathStarted();

        if (enemyRigidbody != null)
        {
            enemyRigidbody.linearVelocity = Vector2.zero;
            enemyRigidbody.simulated = false;
        }

        if (enemyColliders != null)
        {
            foreach (Collider2D enemyCollider in enemyColliders)
            {
                if (enemyCollider != null)
                    enemyCollider.enabled = false;
            }
        }

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
        if (!IsInitialized || IsDestroyed ||
            float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
        {
            return;
        }

        if (!IsDeathAnimationCompleted)
        {
            remainingAnimationTimeout -= deltaTime;
            if (remainingAnimationTimeout > 0f)
                return;

            CompleteDeathAnimation();
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
        enemyColliders = null;

        destroyDelay = 0f;
        destroyTimer = 0f;
        animationTimeout = 0f;
        remainingAnimationTimeout = 0f;

        IsDead = false;
        IsDeathAnimationCompleted = false;
        IsDestroyed = false;
    }

    private static bool IsValidDuration(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
