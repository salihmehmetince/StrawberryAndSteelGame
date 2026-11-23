
using UnityEngine;

public class EnemyAirStateSystem
{
    private Rigidbody2D enemyRigidbody;
    private EnemyGroundDetection groundDetection;

    private const float VerticalSpeedThreshold = 0.05f;

    public bool IsInitialized => enemyRigidbody != null && groundDetection != null;

    public EnemyAirState CurrentState { get; private set; }

    public bool IsGrounded =>
        CurrentState == EnemyAirState.Grounded;

    public bool IsRising =>
        CurrentState == EnemyAirState.Rising;

    public bool IsFalling =>
        CurrentState == EnemyAirState.Falling;

    public bool IsAirborne =>
        CurrentState != EnemyAirState.Grounded;

    public void Initialize(
        Rigidbody2D enemyRigidbody,
        EnemyGroundDetection groundDetection)
    {
        this.enemyRigidbody = enemyRigidbody;
        this.groundDetection = groundDetection;

        CurrentState = EnemyAirState.Grounded;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "EnemyAirStateSystem requires a Rigidbody2D component.");
            return;
        }

        if (groundDetection == null)
        {
            Debug.LogError(
                "EnemyAirStateSystem requires an EnemyGroundDetection instance.");
        }
    }

    public void Update()
    {
        if (enemyRigidbody == null || groundDetection == null)
            return;

        if (groundDetection.IsGrounded)
        {
            CurrentState = EnemyAirState.Grounded;
            return;
        }

        float verticalVelocity = enemyRigidbody.linearVelocity.y;

        if (verticalVelocity > VerticalSpeedThreshold)
        {
            CurrentState = EnemyAirState.Rising;
        }
        else if (verticalVelocity < -VerticalSpeedThreshold)
        {
            CurrentState = EnemyAirState.Falling;
        }
        else
        {
            CurrentState = EnemyAirState.Airborne;
        }
    }

    public void Reset()
    {
        CurrentState = EnemyAirState.Grounded;
    }
}
