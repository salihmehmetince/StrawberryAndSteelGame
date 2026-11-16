
using UnityEngine;

public class EnemyGroundDetection
{
    private Transform groundCheck;
    private LayerMask groundLayer;
    private float groundCheckRadius;

    public bool IsGrounded { get; private set; }

    public void Initialize(
        Transform groundCheck,
        LayerMask groundLayer,
        float groundCheckRadius)
    {
        this.groundCheck = groundCheck;
        this.groundLayer = groundLayer;
        this.groundCheckRadius = groundCheckRadius;

        IsGrounded = false;

        if (groundCheck == null)
        {
            Debug.LogError(
                "EnemyGroundDetection requires a GroundCheck transform.");
            return;
        }

        if (groundCheckRadius <= 0f)
        {
            Debug.LogError(
                "EnemyGroundDetection requires a positive ground check radius.");
            return;
        }

        CheckGround();
    }

    public bool CheckGround()
    {
        if (groundCheck == null || groundCheckRadius <= 0f)
        {
            IsGrounded = false;
            return false;
        }

        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);

        return IsGrounded;
    }

    public void Reset()
    {
        IsGrounded = false;
    }
}
