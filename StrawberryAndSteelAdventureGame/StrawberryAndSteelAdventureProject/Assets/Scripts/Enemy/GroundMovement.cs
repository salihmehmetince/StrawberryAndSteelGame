
using UnityEngine;

public class GroundMovement :
    IEnemyMovementBehaviour,
    IEnemyMovementPhysicsBehaviour,
    IEnemyMovementVelocityReceiver
{
    private Rigidbody2D enemyRigidbody;
    private EnemyAirStateSystem airStateSystem;

    private float movementSpeed;
    private float acceleration;
    private float deceleration;
    private float airAcceleration;
    private float airDeceleration;

    private float currentVelocity;
    private float targetVelocity;

    private float turnDeceleration;
    private float airTurnDeceleration;

    public bool UsesGravity => true;

    public bool IsMoving =>
        Mathf.Abs(currentVelocity) > 0.01f;

    public void Initialize(EnemyController controller)
    {
        enemyRigidbody = controller.EnemyRigidbody;
        airStateSystem = controller.AirStateSystem;

        movementSpeed = controller.Definition.MovementSpeed;
        acceleration = controller.Definition.Acceleration;
        deceleration = controller.Definition.Deceleration;
        airAcceleration = controller.Definition.AirAcceleration;
        airDeceleration = controller.Definition.AirDeceleration;

        turnDeceleration = controller.Definition.TurnDeceleration;
        airTurnDeceleration = controller.Definition.AirTurnDeceleration;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "GroundMovement requires a Rigidbody2D component.");
            return;
        }

        currentVelocity = enemyRigidbody.linearVelocity.x;
        targetVelocity = currentVelocity;
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

    public void SynchronizeVelocity(Vector2 velocity)
    {
        currentVelocity = velocity.x;
    }

    public void Update(float deltaTime)
    {
        if (enemyRigidbody == null)
            return;

        bool isAirborne =
            airStateSystem != null && airStateSystem.IsAirborne;

        float activeAcceleration =
            isAirborne ? airAcceleration : acceleration;

        float activeDeceleration =
            isAirborne ? airDeceleration : deceleration;

        float activeTurnDeceleration =
            isAirborne ? airTurnDeceleration : turnDeceleration;

        bool isStopping = Mathf.Approximately(targetVelocity, 0f);

        bool isChangingDirection =
            !isStopping &&
            !Mathf.Approximately(currentVelocity, 0f) &&
            Mathf.Sign(currentVelocity) != Mathf.Sign(targetVelocity);

        float activeRate;

        if (isStopping)
            activeRate = activeDeceleration;
        else if (isChangingDirection)
            activeRate = activeTurnDeceleration;
        else
            activeRate = activeAcceleration;

        currentVelocity = Mathf.MoveTowards(
            currentVelocity,
            targetVelocity,
            activeRate * deltaTime);

        Vector2 velocity = enemyRigidbody.linearVelocity;
        velocity.x = currentVelocity;

        enemyRigidbody.linearVelocity = velocity;
    }
}
