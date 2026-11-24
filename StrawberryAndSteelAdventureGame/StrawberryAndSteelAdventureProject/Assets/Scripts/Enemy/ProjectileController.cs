
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ProjectileDefinition projectileDefinition;

    private Rigidbody2D rb;

    private Vector2 movementDirection;
    private float currentSpeed;
    private float remainingLifetime;

    private bool isLaunched;
    private bool hasHitPlayer;

    public ProjectileDefinition Definition => projectileDefinition;

    public event Action<ProjectileDefinition, Collider2D> OnPlayerHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isLaunched)
            return;

        UpdateMovementSpeed();
        UpdateLifetime();
    }

    private void FixedUpdate()
    {
        if (!isLaunched)
            return;

        rb.linearVelocity = movementDirection * currentSpeed;
    }

    public void Launch(Vector2 direction)
    {
        if (isLaunched || projectileDefinition == null)
            return;

        movementDirection = direction.normalized;

        currentSpeed = projectileDefinition.MovementSpeed;
        remainingLifetime = projectileDefinition.Lifetime;

        isLaunched = true;

        rb.linearVelocity = movementDirection * currentSpeed;
    }

    private void UpdateMovementSpeed()
    {
        float speedReduction =
            projectileDefinition.SpeedReductionPerSecond * Time.deltaTime;

        currentSpeed = Mathf.Max(0f, currentSpeed - speedReduction);
    }

    private void UpdateLifetime()
    {
        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLaunched || hasHitPlayer)
            return;

        bool hitPlayer = other.CompareTag("Player") ||
            (other.attachedRigidbody != null &&
             other.attachedRigidbody.CompareTag("Player")) ||
            other.transform.root.CompareTag("Player");

        if (!hitPlayer)
            return;

        hasHitPlayer = true;

        OnPlayerHit?.Invoke(projectileDefinition, other);

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
