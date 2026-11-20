using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{
    private EnemyProjectileMovementSystem movementSystem;
    private EnemyProjectileLifetimeSystem lifetimeSystem;
    private EnemyProjectileCollisionSystem collisionSystem;

    private int damage;
    private bool isBeingDestroyed;
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        movementSystem = new EnemyProjectileMovementSystem();
        lifetimeSystem = new EnemyProjectileLifetimeSystem();
    }

    public void Initialize(
        Transform owner,
        int damage,
        Vector2 direction,
        float speed,
        float lifetime)
    {
        IsInitialized = false;
        this.damage = 0;
        collisionSystem = null;

        if (owner == null ||
            damage <= 0 ||
            float.IsNaN(direction.x) ||
            float.IsInfinity(direction.x) ||
            float.IsNaN(direction.y) ||
            float.IsInfinity(direction.y) ||
            direction.sqrMagnitude <= 0f ||
            float.IsInfinity(direction.sqrMagnitude) ||
            float.IsNaN(speed) ||
            float.IsInfinity(speed) ||
            speed < 0f ||
            float.IsNaN(lifetime) ||
            float.IsInfinity(lifetime) ||
            lifetime <= 0f)
        {
            Debug.LogError(
                "EnemyProjectile received invalid initialization values.",
                gameObject);

            return;
        }

        Rigidbody2D projectileRigidbody = GetComponent<Rigidbody2D>();

        if (projectileRigidbody == null ||
            movementSystem == null ||
            lifetimeSystem == null)
        {
            Debug.LogError(
                "EnemyProjectile is missing required components or systems.",
                gameObject);

            return;
        }

        movementSystem.Initialize(projectileRigidbody);
        lifetimeSystem.Initialize(lifetime);
        if (!movementSystem.IsInitialized ||
    !lifetimeSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyProjectile failed to initialize its systems.",
                gameObject);

            return;
        }
        collisionSystem = new EnemyProjectileCollisionSystem();
        collisionSystem.Initialize(owner);
        if (!collisionSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyProjectile failed to initialize its collision system.",
                gameObject);

            return;
        }
        this.damage = damage;

        movementSystem.SetVelocity(direction, speed);

        IsInitialized = true;
    }

    private void Update()
    {
        if (!IsInitialized)
            return;

        lifetimeSystem.Update(Time.deltaTime);

        if (lifetimeSystem.IsExpired)
            DestroyProjectile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInitialized)
            return;

        bool shouldDestroy = collisionSystem.TryHandleCollision(
            other,
            damage,
            transform.position);

        if (shouldDestroy)
            DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (isBeingDestroyed)
            return;

        isBeingDestroyed = true;
        IsInitialized = false;

        Destroy(gameObject);
    }
}