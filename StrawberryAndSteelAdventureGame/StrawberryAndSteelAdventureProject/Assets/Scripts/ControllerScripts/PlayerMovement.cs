
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool wasFacingRight = true;
    private bool wasUpPressed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (inputHandler == null)
            inputHandler = GetComponent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckGround();
        HandleJump();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (rb == null || inputHandler == null)
            return;

        float horizontalInput = inputHandler.MoveInput.x;

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (horizontalInput > 0.01f && !wasFacingRight)
            Flip();

        else if (horizontalInput < -0.01f && wasFacingRight)
            Flip();
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void HandleJump()
    {
        if (inputHandler == null || rb == null)
            return;

        if (inputHandler.ConsumeJump() && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                0f
            );

            rb.AddForce(
                Vector2.up * jumpForce,
                ForceMode2D.Impulse
            );

            isGrounded = false;

        }
    }
    private void UpdateAnimations()
    {
        if (animator == null || rb == null || inputHandler == null)
            return;

        bool isMoving = Mathf.Abs(inputHandler.MoveInput.x) > 0.05f;

        animator.SetBool("IsIdle", !isMoving);
        animator.SetBool("IsRun", isMoving);
    }

    private void Flip()
    {
        wasFacingRight = !wasFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}
