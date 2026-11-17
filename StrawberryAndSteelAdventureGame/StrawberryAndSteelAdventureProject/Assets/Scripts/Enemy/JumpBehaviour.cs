
using UnityEngine;

public class JumpBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;
    private EnemyGroundDetection groundDetection;

    private float jumpForce;
    private float takeoffTimeout;
    private float takeoffTimer;

    private bool hasLeftGround;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Jump;

    public bool IsActive { get; private set; }

    public bool IsGrounded =>
        groundDetection != null && groundDetection.IsGrounded;

    public void Initialize(
        EnemyController controller,
        EnemyGroundDetection groundDetection,
        float jumpForce,
        float takeoffTimeout)
    {
        enemyRigidbody = controller.EnemyRigidbody;
        this.groundDetection = groundDetection;
        this.jumpForce = jumpForce;
        this.takeoffTimeout = takeoffTimeout;

        IsActive = false;
        hasLeftGround = false;
        takeoffTimer = 0f;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "JumpBehaviour requires a Rigidbody2D component.");
            return;
        }

        if (groundDetection == null)
        {
            Debug.LogError(
                "JumpBehaviour requires an EnemyGroundDetection instance.");
        }

        if (takeoffTimeout <= 0f)
        {
            Debug.LogError(
                "JumpBehaviour requires a positive takeoff timeout.");
        }
    }

    public bool StartJump()
    {
        if (enemyRigidbody == null ||
            groundDetection == null ||
            IsActive)
        {
            return false;
        }

        if (!groundDetection.CheckGround() ||
            jumpForce <= 0f ||
            takeoffTimeout <= 0f)
        {
            return false;
        }

        Vector2 velocity = enemyRigidbody.linearVelocity;
        velocity.y = 0f;
        enemyRigidbody.linearVelocity = velocity;

        enemyRigidbody.AddForce(
            Vector2.up * jumpForce,
            ForceMode2D.Impulse);

        IsActive = true;
        hasLeftGround = false;
        takeoffTimer = 0f;

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsActive || groundDetection == null)
            return;

        bool isGrounded = groundDetection.CheckGround();

        if (!hasLeftGround)
        {
            if (!isGrounded)
            {
                hasLeftGround = true;
                return;
            }

            takeoffTimer += deltaTime;

            if (takeoffTimer >= takeoffTimeout)
                Stop();

            return;
        }

        if (isGrounded)
            Stop();
    }

    public void Stop()
    {
        IsActive = false;
        hasLeftGround = false;
        takeoffTimer = 0f;
    }
}
