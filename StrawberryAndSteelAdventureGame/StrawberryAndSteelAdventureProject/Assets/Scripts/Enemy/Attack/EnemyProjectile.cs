using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{
    private EnemyProjectileMovementSystem movementSystem;
    private EnemyProjectileLifetimeSystem lifetimeSystem;
    private EnemyProjectileCollisionSystem collisionSystem;

    private int damage;

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
        if (owner == null ||
            damage <= 0 ||
            direction.sqrMagnitude <= 0f ||
            speed < 0f ||
            lifetime <= 0f)
        {
            Debug.LogError(
                "EnemyProjectile received invalid initialization values.",
                gameObject);

            return;
        }

        Rigidbody2D projectileRigidbody = GetComponent<Rigidbody2D>();

        movementSystem.Initialize(projectileRigidbody);
        lifetimeSystem.Initialize(lifetime);

        collisionSystem = new EnemyProjectileCollisionSystem();
        collisionSystem.Initialize(owner);

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
            Destroy(gameObject);
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
            Destroy(gameObject);
    }
}