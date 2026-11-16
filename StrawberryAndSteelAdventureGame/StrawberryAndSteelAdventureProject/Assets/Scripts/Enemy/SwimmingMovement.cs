
using UnityEngine;

public class SwimmingMovement :
    IEnemyMovementBehaviour,
    IEnemyMovementPhysicsBehaviour
{
    private Rigidbody2D enemyRigidbody;

    private float movementSpeed;
    private float acceleration;

    private Vector2 currentVelocity;
    private Vector2 targetVelocity;

    public bool UsesGravity => false;

    public bool IsMoving =>
        currentVelocity.sqrMagnitude > 0.0001f;

    public void Initialize(EnemyController controller)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        movementSpeed = controller.Definition.MovementSpeed;
        acceleration = controller.Definition.Acceleration;

        currentVelocity = Vector2.zero;
        targetVelocity = Vector2.zero;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "SwimmingMovement requires a Rigidbody2D component.");
        }
    }

    public void Move(Vector2 direction)
    {
        if (enemyRigidbody == null)
            return;

        targetVelocity = direction.normalized * movementSpeed;
    }

    public void Stop()
    {
        targetVelocity = Vector2.zero;
    }

    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void Update(float deltaTime)
    {
        if (enemyRigidbody == null)
            return;

        currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * deltaTime);

        enemyRigidbody.linearVelocity = currentVelocity;
    }
}
