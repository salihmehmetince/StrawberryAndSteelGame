
using UnityEngine;

public class EnemyPhysicsSettings
{
    private Rigidbody2D enemyRigidbody;
    private float originalGravityScale;

    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    public void Initialize(Rigidbody2D enemyRigidbody)
    {
        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "EnemyPhysicsSettings requires a Rigidbody2D component.");
            return;
        }

        this.enemyRigidbody = enemyRigidbody;

        originalGravityScale = enemyRigidbody.gravityScale;

        isInitialized = true;
    }

    public void ApplyMovementPhysics(
        IEnemyMovementPhysicsBehaviour behaviour)
    {
        if (!isInitialized || behaviour == null)
            return;

        enemyRigidbody.gravityScale = behaviour.UsesGravity
            ? originalGravityScale
            : 0f;
    }

    public void RestoreOriginalSettings()
    {
        if (!isInitialized)
            return;

        enemyRigidbody.gravityScale = originalGravityScale;
    }
}
