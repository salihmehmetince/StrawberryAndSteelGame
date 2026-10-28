
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayerDamageable
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

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
    [SerializeField] private float attackOffset = 0.6f;
    [SerializeField] private int attackDamage = 1;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 1.2f;
    [SerializeField] private float interactDuration = 0.5f;

    [Header("Hurt")]
    [SerializeField] private float hurtDuration = 0.35f;

    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private int currentHealth;

    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isAttacking;
    private bool isDashing;
    private bool isInteracting;
    private bool isHurting;
    private bool isDead;

    private bool wasUpHeld;

    private float attackTimer;
    private float dashTimer;
    private float interactTimer;
    private float hurtTimer;
    private float dashDirection;

    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    public bool IsDefending { get; private set; }

    private static readonly int IsIdleHash =
        Animator.StringToHash("IsIdle");

    private static readonly int IsRunHash =
        Animator.StringToHash("IsRun");

    private static readonly int IsHurtHash =
        Animator.StringToHash("IsHurt");

    private static readonly int IsDeathHash =
        Animator.StringToHash("IsDeath");

    private static readonly int IsAttackHash =
        Animator.StringToHash("IsAttack");

    private static readonly int IsDashHash =
        Animator.StringToHash("IsDash");

    private static readonly int IsDefendHash =
        Animator.StringToHash("IsDefend");

    private static readonly int IsInteractHash =
        Animator.StringToHash("IsInteract");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (inputHandler == null)
            inputHandler = GetComponent<PlayerInputHandler>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;

        if (rb == null)
            Debug.LogError("PlayerMovement: Rigidbody2D bulunamadý.", this);

        if (inputHandler == null)
            Debug.LogError("PlayerMovement: PlayerInputHandler bulunamadý.", this);

        if (animator == null)
            Debug.LogWarning("PlayerMovement: Animator bulunamadý. Hareket yine çalýþabilir.", this);
    }

    private void Update()
    {
        if (inputHandler == null)
            return;

        if (isDead)
        {
            UpdateAnimations();
            return;
        }

        CheckGround();
        HandleJump();
        HandleActions();
        UpdateTimers();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (rb == null || inputHandler == null)
            return;

        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Dash sýrasýnda yatay hýz her fizik karesinde korunur.
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(
                dashDirection * dashSpeed,
                rb.linearVelocity.y
            );

            return;
        }

        float horizontalInput = inputHandler.MoveInput.x;

        if (IsDefending || isHurting || isInteracting)
            horizontalInput = 0f;

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (horizontalInput > 0.01f && !isFacingRight)
            Flip();
        else if (horizontalInput < -0.01f && isFacingRight)
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

        // Yukarý girdisi basýlý tutulurken yalnýzca ilk basýþta zýpla.
        if (upHeld &&
            !wasUpHeld &&
            isGrounded &&
            !isDashing &&
            !IsDefending &&
            !isAttacking &&
            !isHurting &&
            !isInteracting)
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
        bool attackPressed = inputHandler.ConsumeAttack();
        bool dashPressed = inputHandler.ConsumeDash();
        bool interactPressed = inputHandler.ConsumeInteract();
        bool pausePressed = inputHandler.ConsumePause();

        if (pausePressed)
            TogglePause();

        IsDefending =
            inputHandler.IsDefending &&
            !isAttacking &&
            !isDashing &&
            !isInteracting &&
            !isHurting;

        if (isDead || IsDefending || isHurting)
            return;

        // Ayný anda birden fazla aksiyon baþlatýlmasýný engelle.
        if (isAttacking || isDashing || isInteracting)
            return;

        if (attackPressed)
            StartAttack();
        else if (dashPressed)
            StartDash();
        else if (interactPressed)
            TryInteract();
    }

    private void StartAttack()
    {
        if (isDead || isAttacking || isDashing ||
            isInteracting || isHurting || IsDefending)
            return;

        isAttacking = true;
        attackTimer = attackDuration;

        PerformAttack();
    }

    private void PerformAttack()
    {
        float direction = isFacingRight ? 1f : -1f;

        Vector2 attackPosition =
            (Vector2)transform.position +
            Vector2.right * direction * attackOffset;

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
        if (isDead || isDashing || isAttacking ||
            isInteracting || isHurting || IsDefending)
            return;

        float horizontalInput = inputHandler.MoveInput.x;

        if (Mathf.Abs(horizontalInput) > 0.01f)
            dashDirection = Mathf.Sign(horizontalInput);
        else
            dashDirection = isFacingRight ? 1f : -1f;

        isDashing = true;
        dashTimer = dashDuration;

        // Hemen hýz ver; FixedUpdate da bu hýzý koruyacak.
        rb.linearVelocity = new Vector2(
            dashDirection * dashSpeed,
            rb.linearVelocity.y
        );
    }

    private void TryInteract()
    {
        if (isDead || isAttacking || isDashing ||
            isInteracting || isHurting || IsDefending)
            return;

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

        // Uygun etkileþim nesnesi yoksa animasyon baþlatma.
        if (nearest == null)
            return;

        isInteracting = true;
        interactTimer = interactDuration;

        nearest.Interact(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        isAttacking = false;
        isDashing = false;
        isInteracting = false;
        IsDefending = false;

        isHurting = true;
        hurtTimer = hurtDuration;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    private void Die()
    {
        isDead = true;

        isAttacking = false;
        isDashing = false;
        isInteracting = false;
        isHurting = false;
        IsDefending = false;

        rb.linearVelocity = Vector2.zero;
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

        if (isInteracting)
        {
            interactTimer -= Time.deltaTime;

            if (interactTimer <= 0f)
                isInteracting = false;
        }

        if (isHurting)
        {
            hurtTimer -= Time.deltaTime;

            if (hurtTimer <= 0f)
                isHurting = false;
        }
    }

    private void UpdateAnimations()
    {
        // Animator eksikliði hareketi veya girdileri durdurmamalý.
        if (animator == null)
            return;

        if (isDead)
        {
            animator.SetBool(IsIdleHash, false);
            animator.SetBool(IsRunHash, false);
            animator.SetBool(IsHurtHash, false);
            animator.SetBool(IsDeathHash, true);
            animator.SetBool(IsAttackHash, false);
            animator.SetBool(IsDashHash, false);
            animator.SetBool(IsDefendHash, false);
            animator.SetBool(IsInteractHash, false);
            return;
        }

        bool isMoving =
            !IsDefending &&
            !isAttacking &&
            !isDashing &&
            !isInteracting &&
            !isHurting &&
            Mathf.Abs(inputHandler.MoveInput.x) > 0.05f;

        bool isBusy =
            isAttacking ||
            isDashing ||
            isInteracting ||
            isHurting ||
            IsDefending;

        animator.SetBool(IsIdleHash, !isMoving && !isBusy);
        animator.SetBool(IsRunHash, isMoving);

        animator.SetBool(IsHurtHash, isHurting);
        animator.SetBool(IsDeathHash, false);
        animator.SetBool(IsAttackHash, isAttacking);
        animator.SetBool(IsDashHash, isDashing);
        animator.SetBool(IsDefendHash, IsDefending);
        animator.SetBool(IsInteractHash, isInteracting);
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
            transform.position +
            Vector3.right * direction * attackOffset,
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
