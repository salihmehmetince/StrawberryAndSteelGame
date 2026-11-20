
using UnityEngine;

public class EnemyProjectileCollisionSystem
{
    private Transform projectileOwner;
    private bool hasCollided;

    public bool IsInitialized => projectileOwner != null;

    public bool HasCollided => hasCollided;

    public void Initialize(Transform projectileOwner)
    {
        this.projectileOwner = null;
        hasCollided = false;

        if (projectileOwner == null)
        {
            Debug.LogError(
                "EnemyProjectileCollisionSystem requires a projectile owner.");

            return;
        }

        this.projectileOwner = projectileOwner;
    }
    public bool TryHandleCollision(
        Collider2D other,
        int damage,
        Vector2 attackPosition)
    {
        if (!IsInitialized || hasCollided || other == null || damage <= 0)
            return false;

        if (other.transform.root == projectileOwner.root)
            return false;

        hasCollided = true;

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return true;

        damageable.ReceiveDamage(
            damage,
            attackPosition);

        return true;
    }
}
