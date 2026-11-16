
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyDefinition definition;

    [Header("Jump Configuration")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float jumpForce;

    private Rigidbody2D enemyRigidbody;
    private Collider2D mainCollider;
    private Animator animator;
    private SpriteRenderer[] spriteRenderers;

    private EnemyStateSystem stateSystem;
    private EnemyMovementSystem movementSystem;

    private EnemyGroundDetection groundDetection;
    private EnemyAirStateSystem airStateSystem;
    private JumpBehaviour jumpBehaviour;

    private bool isInitialized;

    public EnemyDefinition Definition => definition;

    public Rigidbody2D EnemyRigidbody => enemyRigidbody;
    public Collider2D MainCollider => mainCollider;
    public Animator Animator => animator;
    public SpriteRenderer[] SpriteRenderers => spriteRenderers;

    public EnemyStateSystem StateSystem => stateSystem;
    public EnemyMovementSystem MovementSystem => movementSystem;

    public EnemyGroundDetection GroundDetection => groundDetection;
    public EnemyAirStateSystem AirStateSystem => airStateSystem;

    public bool IsInitialized => isInitialized;

    public bool IsJumping =>
        movementSystem != null && movementSystem.IsJumping;

    public bool IsGrounded =>
        airStateSystem != null && airStateSystem.IsGrounded;

    public bool IsRising =>
        airStateSystem != null && airStateSystem.IsRising;

    public bool IsFalling =>
        airStateSystem != null && airStateSystem.IsFalling;

    public bool IsAirborne =>
        airStateSystem != null && airStateSystem.IsAirborne;

    public EnemyAirState CurrentAirState =>
        airStateSystem != null
            ? airStateSystem.CurrentState
            : EnemyAirState.Grounded;

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (!isInitialized)
            return;

        groundDetection.CheckGround();

        airStateSystem.Update();

        movementSystem.Update(Time.fixedDeltaTime);
    }

    public void Initialize()
    {
        if (isInitialized)
            return;

        if (definition == null)
        {
            Debug.LogError(
                $"EnemyDefinition is missing on {gameObject.name}.",
                gameObject);
            return;
        }

        CacheComponents();

        stateSystem = new EnemyStateSystem();
        movementSystem = new EnemyMovementSystem(this);

        InitializeGroundDetection();
        InitializeAirStateSystem();
        InitializeJumpBehaviour();

        isInitialized = true;
    }

    private void CacheComponents()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>(true);
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void InitializeGroundDetection()
    {
        groundDetection = new EnemyGroundDetection();

        groundDetection.Initialize(
            groundCheck,
            groundLayer,
            groundCheckRadius);
    }

    private void InitializeAirStateSystem()
    {
        airStateSystem = new EnemyAirStateSystem();

        airStateSystem.Initialize(
            enemyRigidbody,
            groundDetection);

        airStateSystem.Update();
    }

    private void InitializeJumpBehaviour()
    {
        jumpBehaviour = new JumpBehaviour();

        jumpBehaviour.Initialize(
            this,
            groundDetection,
            jumpForce);

        movementSystem.SetJumpBehaviour(jumpBehaviour);
    }

    // Normal Movement

    public void SetMovementBehaviour(IEnemyMovementBehaviour behaviour)
    {
        if (!isInitialized)
            return;

        movementSystem.SetBehaviour(behaviour);
    }

    public void Move(Vector2 direction)
    {
        if (!isInitialized)
            return;

        movementSystem.Move(direction);
    }

    public void StopMovement()
    {
        if (!isInitialized)
            return;

        movementSystem.Stop();
    }

    public void StopNormalMovement()
    {
        if (!isInitialized)
            return;

        movementSystem.StopNormalMovement();
    }

    public void SetMovementSpeed(float speed)
    {
        if (!isInitialized)
            return;

        movementSystem.SetSpeed(speed);
    }

    // Jump

    public bool Jump()
    {
        if (!isInitialized)
            return false;

        return movementSystem.Jump();
    }
}
