
using UnityEngine;

public class GroundMovement :
    IEnemyMovementBehaviour,
    IEnemyMovementPhysicsBehaviour
{
    private Rigidbody2D enemyRigidbody;

    private float movementSpeed;
    private float acceleration;

    private float currentVelocity;
    private float targetVelocity;

    public bool UsesGravity => true;

    public bool IsMoving =>
        Mathf.Abs(currentVelocity) > 0.01f;

    public void Initialize(EnemyController controller)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        movementSpeed = controller.Definition.MovementSpeed;
        acceleration = controller.Definition.Acceleration;

        currentVelocity = 0f;
        targetVelocity = 0f;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "GroundMovement requires a Rigidbody2D component.");
        }
    }

    public void Move(Vector2 direction)
    {
        if (enemyRigidbody == null)
            return;

        float horizontalDirection =
            Mathf.Clamp(direction.x, -1f, 1f);

        targetVelocity = horizontalDirection * movementSpeed;
    }

    public void Stop()
    {
        targetVelocity = 0f;
    }

    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void Update(float deltaTime)
    {
        if (enemyRigidbody == null)
            return;

        currentVelocity = Mathf.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * deltaTime);

        Vector2 velocity = enemyRigidbody.linearVelocity;
        velocity.x = currentVelocity;

        enemyRigidbody.linearVelocity = velocity;
    }
}
