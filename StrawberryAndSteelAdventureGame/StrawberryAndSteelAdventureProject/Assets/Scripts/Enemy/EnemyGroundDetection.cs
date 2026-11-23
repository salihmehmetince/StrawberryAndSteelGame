
using UnityEngine;

public class EnemyGroundDetection
{
    private Transform groundCheck;
    private LayerMask groundLayer;
    private float groundCheckRadius;

    public bool IsGrounded { get; private set; }

    public bool IsInitialized => groundCheck != null && groundCheckRadius > 0f;

    public void Initialize(
        Transform groundCheck,
        LayerMask groundLayer,
        float groundCheckRadius)
    {
        this.groundCheck = null;
        this.groundLayer = default;
        this.groundCheckRadius = 0f;

        if (groundCheck == null ||
            float.IsNaN(groundCheckRadius) ||
            float.IsInfinity(groundCheckRadius) ||
            groundCheckRadius <= 0f ||
            groundLayer.value == 0)
        {
            IsGrounded = false;
            Debug.LogError("EnemyGroundDetection requires a GroundCheck transform, a positive finite radius, and a non-empty ground layer.");
            return;
        }

        this.groundCheck = groundCheck;
        this.groundLayer = groundLayer;
        this.groundCheckRadius = groundCheckRadius;

        IsGrounded = false;

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
