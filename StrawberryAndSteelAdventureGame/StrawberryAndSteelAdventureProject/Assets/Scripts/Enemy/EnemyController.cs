
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private EnemyDefinition definition;

    [Header("Visual Configuration")]
    [SerializeField] private Transform visualRoot;

    [Header("Jump Configuration")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float jumpForce;
    [SerializeField] private float takeoffTimeout;

    [Header("Detection Configuration")]
    [SerializeField] private Transform detectionOrigin;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Target Scanning")]
    [SerializeField] private float targetScanRadius;

    private Rigidbody2D enemyRigidbody;
    private Collider2D mainCollider;
    private Animator animator;
    private SpriteRenderer[] spriteRenderers;

    private EnemyStateSystem stateSystem;
    private EnemyMovementSystem movementSystem;
    private EnemyFacingSystem facingSystem;

    private EnemyGroundDetection groundDetection;
    private EnemyAirStateSystem airStateSystem;
    private JumpBehaviour jumpBehaviour;

    private bool isInitialized;

    private EnemyTargetScanner targetScanner;

    private EnemyDetectionSystem detectionSystem;

    private EnemyTargetSystem targetSystem;

    private IEnemyTargetSearchBehaviour targetSearchBehaviour;
    public EnemyDefinition Definition => definition;

    public Rigidbody2D EnemyRigidbody => enemyRigidbody;
    public Collider2D MainCollider => mainCollider;
    public Animator Animator => animator;
    public SpriteRenderer[] SpriteRenderers => spriteRenderers;

    public EnemyStateSystem StateSystem => stateSystem;
    public EnemyMovementSystem MovementSystem => movementSystem;
    public EnemyFacingSystem FacingSystem => facingSystem;

    public EnemyGroundDetection GroundDetection => groundDetection;
    public EnemyAirStateSystem AirStateSystem => airStateSystem;

    public bool IsInitialized => isInitialized;

    public EnemyDetectionSystem DetectionSystem => detectionSystem;

    public EnemyTargetScanner TargetScanner => targetScanner;


    public EnemyTargetSystem TargetSystem => targetSystem;

    public IEnemyTargetSearchBehaviour TargetSearchBehaviour =>
    targetSearchBehaviour;


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

    public IEnemyMovementBehaviour ActiveMovementBehaviour =>
        movementSystem != null ? movementSystem.ActiveBehaviour : null;

    public IEnemyMovementAction ActiveMovementAction =>
        movementSystem != null ? movementSystem.ActiveAction : null;

    public bool IsNormalMovementActive =>
        movementSystem != null && movementSystem.IsNormalMovementActive;

    public bool IsSpecialMovementActive =>
        movementSystem != null && movementSystem.IsSpecialMovementActive;

    public bool IsMovementLocked =>
        movementSystem != null && movementSystem.IsLocked;

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        groundDetection.CheckGround();
        airStateSystem.Update();
        movementSystem.Update(Time.fixedDeltaTime);
        targetSearchBehaviour?.Update(Time.fixedDeltaTime);
    }

    public void Initialize()
    {
        if (isInitialized) return;

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
        InitializeFacingSystem();
        InitializeDetectionSystem();
        InitializeTargetScanner();

        targetSystem = new EnemyTargetSystem();

        InitializeTargetSearchBehaviour();

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
        airStateSystem.Initialize(enemyRigidbody, groundDetection);
        airStateSystem.Update();
    }

    private void InitializeJumpBehaviour()
    {
        jumpBehaviour = new JumpBehaviour();
        jumpBehaviour.Initialize(
            this,
            groundDetection,
            jumpForce,
            takeoffTimeout);

        movementSystem.SetJumpBehaviour(jumpBehaviour);
    }

    private void InitializeFacingSystem()
    {
        facingSystem = new EnemyFacingSystem();
        facingSystem.Initialize(visualRoot);
    }

    // Facing API

    public void Face(EnemyFacingDirection direction)
    {
        if (!isInitialized) return;
        facingSystem.Face(direction);
    }

    public void FaceLeft()
    {
        Face(EnemyFacingDirection.Left);
    }

    public void FaceRight()
    {
        Face(EnemyFacingDirection.Right);
    }

    public void FaceTarget(Transform target)
    {
        if (!isInitialized || target == null) return;

        Vector2 currentPosition = enemyRigidbody != null
            ? enemyRigidbody.position
            : (Vector2)transform.position;

        facingSystem.FaceTarget(target.position, currentPosition);
    }

    // Movement API

    public void SetMovementBehaviour(IEnemyMovementBehaviour behaviour)
    {
        if (!isInitialized) return;
        movementSystem.SetBehaviour(behaviour);
    }

    public void Move(Vector2 direction)
    {
        if (!isInitialized) return;
        movementSystem.Move(direction);
    }

    public void StopMovement()
    {
        if (!isInitialized) return;
        movementSystem.Stop();
    }

    public void StopNormalMovement()
    {
        if (!isInitialized) return;
        movementSystem.StopNormalMovement();
    }

    public void SetMovementSpeed(float speed)
    {
        if (!isInitialized) return;
        movementSystem.SetSpeed(speed);
    }

    public bool Jump()
    {
        if (!isInitialized) return false;
        return movementSystem.Jump();
    }

    public bool TryActivateMovementAction(
        IEnemyMovementAction action,
        System.Func<bool> startAction)
    {
        if (!isInitialized) return false;

        return movementSystem.TryActivateAction(action, startAction);
    }

    public bool StopMovementAction(IEnemyMovementAction action)
    {
        if (!isInitialized) return false;
        return movementSystem.StopAction(action);
    }

    public bool IsMovementActionActive(IEnemyMovementAction action)
    {
        if (!isInitialized) return false;
        return movementSystem.IsActionActive(action);
    }

    public void StopActiveMovementAction()
    {
        if (!isInitialized) return;
        movementSystem.StopActiveAction();
    }

    public void LockMovement()
    {
        if (!isInitialized) return;
        movementSystem.Lock();
    }

    public void UnlockMovement()
    {
        if (!isInitialized) return;
        movementSystem.Unlock();
    }


    public bool IsFacing(EnemyFacingDirection direction)
    {
        if (!isInitialized) return false;

        return facingSystem.IsFacing(direction);
    }

    private void InitializeDetectionSystem()
    {
        detectionSystem = new EnemyDetectionSystem();

        EnemyDetectionSettings settings = new EnemyDetectionSettings(
            definition.DetectionRange,
            definition.VisionAngle,
            targetLayers,
            obstacleLayers);

        detectionSystem.Initialize(
            detectionOrigin,
            visualRoot,
            settings);
    }

    private void InitializeTargetScanner()
    {
        targetScanner = new EnemyTargetScanner();

        float scanRadius = Mathf.Max(
            targetScanRadius,
            definition.DetectionRange);

        targetScanner.Initialize(
            detectionSystem,
            detectionOrigin,
            scanRadius,
            targetLayers);
    }
    public bool CanSeeTarget(Transform target)
    {
        if (!isInitialized || detectionSystem == null)
            return false;

        return detectionSystem.CanSeeTarget(target);
    }

    public bool IsTargetInRange(Transform target)
    {
        if (!isInitialized || detectionSystem == null)
            return false;

        return detectionSystem.IsTargetInRange(target);
    }

    public bool IsTargetInVisionAngle(Transform target)
    {
        if (!isInitialized || detectionSystem == null)
            return false;

        return detectionSystem.IsTargetInVisionAngle(target);
    }

    public bool HasLineOfSight(Transform target)
    {
        if (!isInitialized || detectionSystem == null)
            return false;

        return detectionSystem.HasLineOfSight(target);
    }

    public Transform FindNearestVisibleTarget()
    {
        if (!isInitialized || targetScanner == null)
            return null;

        return targetScanner.FindNearestVisibleTarget();
    }


    public bool SetTarget(Transform target)
    {
        return targetSystem != null &&
               targetSystem.SetTarget(target);
    }

    public bool StartTrackingTarget()
    {
        return targetSystem != null &&
               targetSystem.StartTracking();
    }

    public bool ReacquireTarget()
    {
        return targetSystem != null &&
               targetSystem.ReacquireTarget();
    }
    public void UpdateLastKnownTargetPosition()
    {
        targetSystem?.UpdateLastKnownPosition();
    }

    public bool LoseTarget()
    {
        return targetSystem != null &&
               targetSystem.LoseTarget();
    }

    public bool StartSearchingForTarget()
    {
        return targetSystem != null &&
               targetSystem.StartSearching();
    }

    public void ClearTarget()
    {
        targetSystem?.ClearTarget();
    }


    public bool CanCurrentlySeeTarget(Transform target)
    {
        return detectionSystem != null &&
               detectionSystem.CanSeeTarget(target);
    }


    public bool CompleteTargetSearching()
    {
        return targetSystem != null &&
               targetSystem.CompleteSearching();
    }


    public void InitializeTargetSearchBehaviour(
        IEnemyTargetSearchBehaviour behaviour,
        float horizontalArrivalDistance,
        float verticalArrivalDistance,
        float searchSpeed)
    {
        if (behaviour == null)
        {
            Debug.LogError(
                "EnemyController requires a Target Search Behaviour.");

            return;
        }

        targetSearchBehaviour = behaviour;

        targetSearchBehaviour.Initialize(
            this,
            horizontalArrivalDistance,
            verticalArrivalDistance,
            searchSpeed);
    }


    public void StartTargetSearching(Vector2 targetPosition)
    {
        targetSearchBehaviour?.StartSearching(targetPosition);
    }

    public void UpdateTargetSearchPosition(Vector2 targetPosition)
    {
        targetSearchBehaviour?.SetSearchPosition(targetPosition);
    }

    public void StopTargetSearching()
    {
        targetSearchBehaviour?.StopSearching();
    }

    public bool IsTargetSearchBehaviourActive()
    {
        return targetSearchBehaviour != null &&
               targetSearchBehaviour.IsSearching;
    }

    private void InitializeTargetSearchBehaviour()
    {
        GroundTargetSearchBehaviour behaviour =
            new GroundTargetSearchBehaviour();

        InitializeTargetSearchBehaviour(
            behaviour,
            definition.TargetSearchArrivalDistance,
            definition.TargetSearchVerticalArrivalDistance,
            definition.TargetSearchSpeed);
    }

}
