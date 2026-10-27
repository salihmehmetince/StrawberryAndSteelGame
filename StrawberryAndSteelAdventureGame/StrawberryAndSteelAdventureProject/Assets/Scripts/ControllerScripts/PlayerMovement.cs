
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

    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.35f;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private int attackDamage = 1;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 1.2f;

    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isAttacking;
    private bool isDashing;
    private bool wasUpHeld;

    private float attackTimer;
    private float dashTimer;
    private float dashDirection;

    public bool IsDefending { get; private set; }

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
        if (inputHandler == null || animator == null)
            return;

        CheckGround();
        HandleJump();
        HandleActions();
        UpdateTimers();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (inputHandler == null)
            return;

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(
                dashDirection * dashSpeed,
                rb.linearVelocity.y
            );

            return;
        }

        float horizontalInput = IsDefending
            ? 0f
            : inputHandler.MoveInput.x;

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (horizontalInput > 0f && !isFacingRight)
            Flip();

        else if (horizontalInput < 0f && isFacingRight)
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
        bool upHeld = inputHandler.MoveInput.y > 0.5f;

        // Yukarý girdisi yalnýzca ilk basýþta zýplatýr.
        // Tuþ basýlý tutulduðunda sürekli yeniden zýplamaz.
        if (upHeld && !wasUpHeld &&
            isGrounded && !isDashing && !IsDefending)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        wasUpHeld = upHeld;
    }

    private void HandleActions()
    {
        IsDefending = inputHandler.IsDefending;

        if (inputHandler.ConsumeAttack())
        {
            if (!IsDefending && !isDashing)
                StartAttack();
        }

        if (inputHandler.ConsumeDash())
        {
            if (!IsDefending)
                StartDash();
        }

        if (inputHandler.ConsumeInteract())
        {
            TryInteract();
        }

        if (inputHandler.ConsumePause())
        {
            TogglePause();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        // IsAttack bir Bool parametresidir.
        animator.SetBool("IsAttack", true);

        PerformAttack();
    }

    private void PerformAttack()
    {
        float direction = isFacingRight ? 1f : -1f;

        Vector2 attackPosition = (Vector2)transform.position
            + Vector2.right * direction * 0.6f;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRange
        );

        foreach (Collider2D target in targets)
        {
            if (target.transform.root == transform.root)
                continue;

            IPlayerDamageable damageable =
                target.GetComponentInParent<IPlayerDamageable>();

            if (damageable != null)
                damageable.TakeDamage(attackDamage);
        }
    }

    private void StartDash()
    {
        if (isDashing)
            return;

        float horizontalInput = inputHandler.MoveInput.x;

        if (Mathf.Abs(horizontalInput) > 0.01f)
            dashDirection = Mathf.Sign(horizontalInput);
        else
            dashDirection = isFacingRight ? 1f : -1f;

        isDashing = true;
        dashTimer = dashDuration;

        rb.linearVelocity = new Vector2(
            dashDirection * dashSpeed,
            rb.linearVelocity.y
        );
    }

    private void TryInteract()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRange
        );

        IPlayerInteractable nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D target in nearbyObjects)
        {
            if (target.transform.root == transform.root)
                continue;

            IPlayerInteractable interactable =
                target.GetComponentInParent<IPlayerInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                target.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = interactable;
            }
        }

        if (nearest != null)
            nearest.Interact(gameObject);
    }

    private void TogglePause()
    {
        Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
    }

    private void UpdateTimers()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
                isAttacking = false;
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
                isDashing = false;
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = !IsDefending &&
            Mathf.Abs(rb.linearVelocity.x) > 0.05f;

        // Idle ve Run birbirini dýþlar.
        // Saldýrý sýrasýnda ikisi de false olur.
        animator.SetBool("IsIdle", !isMoving && !isAttacking);
        animator.SetBool("IsRun", isMoving && !isAttacking);
        animator.SetBool("IsAttack", isAttacking);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDisable()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        float direction = isFacingRight ? 1f : -1f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + Vector3.right * direction * 0.6f,
            attackRange
        );

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position,
            interactionRange
        );
    }
}

public interface IPlayerDamageable
{
    void TakeDamage(int damage);
}

public interface IPlayerInteractable
{
    void Interact(GameObject interactor);
}
