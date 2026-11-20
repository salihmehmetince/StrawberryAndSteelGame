using UnityEngine;

public class EnemyProjectileMovementSystem
{
    private Rigidbody2D projectileRigidbody;

    public bool IsInitialized => projectileRigidbody != null;

    public void Initialize(
        Rigidbody2D projectileRigidbody)
    {
        this.projectileRigidbody = null;

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
        if (!IsInitialized ||
            float.IsNaN(direction.x) ||
            float.IsInfinity(direction.x) ||
            float.IsNaN(direction.y) ||
            float.IsInfinity(direction.y) ||
            direction.sqrMagnitude <= 0f ||
            float.IsInfinity(direction.sqrMagnitude) ||
            float.IsNaN(speed) ||
            float.IsInfinity(speed) ||
            speed < 0f)
        {
            return;
        }

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