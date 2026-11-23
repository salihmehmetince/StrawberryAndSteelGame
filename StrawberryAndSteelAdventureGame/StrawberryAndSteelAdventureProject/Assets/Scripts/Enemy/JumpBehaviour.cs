
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
    public bool IsInitialized { get; private set; }

    public bool IsGrounded =>
        groundDetection != null && groundDetection.IsGrounded;

    public void Initialize(
        EnemyController controller,
        EnemyGroundDetection groundDetection,
        float jumpForce,
        float takeoffTimeout)
    {
        enemyRigidbody = null;
        this.groundDetection = groundDetection;
        this.jumpForce = 0f;
        this.takeoffTimeout = 0f;

        IsActive = false;
        IsInitialized = false;
        hasLeftGround = false;
        takeoffTimer = 0f;

        if (controller == null || controller.EnemyRigidbody == null ||
            groundDetection == null || !groundDetection.IsInitialized ||
            !IsFinite(jumpForce) || jumpForce <= 0f ||
            !IsFinite(takeoffTimeout) || takeoffTimeout <= 0f)
        {
            Debug.LogError("JumpBehaviour requires a Rigidbody2D, initialized ground detection, and positive finite jump settings.");
            return;
        }

        enemyRigidbody = controller.EnemyRigidbody;
        this.jumpForce = jumpForce;
        this.takeoffTimeout = takeoffTimeout;
        IsInitialized = true;
    }

    public bool StartJump()
    {
        if (!IsInitialized || enemyRigidbody == null ||
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
        if (!IsInitialized || !IsActive || groundDetection == null ||
            !IsFinite(deltaTime) || deltaTime < 0f)
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

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
