
using UnityEngine;

public class EnemyProjectileMovementSystem
{
    private Rigidbody2D projectileRigidbody;

    public bool IsInitialized => projectileRigidbody != null;

    public void Initialize(
        Rigidbody2D projectileRigidbody)
    {
        if (projectileRigidbody == null)
        {
            Debug.LogError(
                "EnemyProjectileMovementSystem requires a Rigidbody2D.");
            return;
        }

        this.projectileRigidbody = projectileRigidbody;
    }

    public void SetVelocity(Vector2 direction, float speed)
    {
        if (!IsInitialized || direction.sqrMagnitude <= 0f || speed < 0f)
            return;

        projectileRigidbody.linearVelocity =
            direction.normalized * speed;
    }

    public void Stop()
    {
        if (!IsInitialized)
            return;

        projectileRigidbody.linearVelocity = Vector2.zero;
    }
}
