
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
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

    [Header("Attack Configuration")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private EnemyProjectile projectilePrefab;

    [Header("Target Scanning")]
    [SerializeField] private float targetScanRadius;

    [Header("Hurt Configuration")]
    [SerializeField] private float knockbackForce;
    [SerializeField] private float verticalKnockbackForce;
    [SerializeField, Min(0.01f)] private float knockbackDuration = 0.2f;
    [SerializeField, Min(0.01f)] private float hurtAnimationFallbackDuration = 1f;
    [SerializeField, Min(0f)] private float attackAnimationEventFallbackGraceDuration = 0.5f;

    [Header("Death Configuration")]
    [SerializeField] private float deathDestroyDelay;
    [SerializeField, Min(0.01f)] private float deathAnimationTimeout = 5f;

    [Header("Feedback Configuration")]
    [SerializeField] private AudioSource feedbackAudioSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private ParticleSystem hurtEffect;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private ParticleSystem healEffect;

    [Header("Dialogue Configuration")]
    [SerializeField] private AudioSource dialogueAudioSource;

    // Normal Attack Systems

    private EnemyAttackSystem attackSystem;
    private EnemyAttackTargetSystem attackTargetSystem;
    private EnemyAttackDamageSystem attackDamageSystem;
    private EnemyAttackLineOfSightSystem attackLineOfSightSystem;
    private EnemyAttackValidationSystem attackValidationSystem;
    private EnemyMeleeHitDetectionSystem meleeHitDetectionSystem;

    private EnemyAttackCooldownSystem attackCooldownSystem;
    private EnemyAttackStateSystem attackStateSystem;
    private EnemyAttackTimingSystem attackTimingSystem;
    private EnemyAttackSequenceSystem attackSequenceSystem;

    private EnemyAttackBehaviourSystem attackBehaviourSystem;
    private EnemyAttackBehaviourFactory attackBehaviourFactory;

    // Special Attack Systems

    private EnemySpecialAttackSystem specialAttackSystem;
    private EnemyAttackStateSystem specialAttackStateSystem;
    private EnemySpecialAttackSequenceSystem specialAttackSequenceSystem;
    private EnemySpecialAttackBehaviourFactory specialAttackBehaviourFactory;

    // Attack Event Handlers

    private EnemyAttackAnimationEventHandler attackAnimationEventHandler;
    private EnemyAttackHitEventHandler attackHitEventHandler;

    // Components

    private Rigidbody2D enemyRigidbody;
    private Collider2D mainCollider;
    private Collider2D[] enemyColliders;
    private Animator animator;
    private SpriteRenderer[] spriteRenderers;

    // Animation

    private EnemyAnimationController animationController;
    private EnemyAnimationEventRelay animationEventRelay;

    // Movement Systems

    private EnemyStateSystem stateSystem;
    private EnemyMovementSystem movementSystem;
    private EnemyFacingSystem facingSystem;

    private EnemyGroundDetection groundDetection;
    private EnemyAirStateSystem airStateSystem;
    private JumpBehaviour jumpBehaviour;
    private DashBehaviour dashBehaviour;
    private ChargeBehaviour chargeBehaviour;

    // Detection and Target Systems

    private EnemyTargetScanner targetScanner;
    private EnemyDetectionSystem detectionSystem;
    private EnemyTargetSystem targetSystem;
    private IEnemyTargetSearchBehaviour targetSearchBehaviour;

    // Health Systems

    private EnemyHealthSystem healthSystem;
    private EnemyDamageSystem damageSystem;
    private EnemyHurtSystem hurtSystem;
    private EnemyDeathSystem deathSystem;
    private EnemyFeedbackSystem feedbackSystem;
    private EnemyHealthEventHandler healthEventHandler;
    private KnockbackBehaviour knockbackBehaviour;

    // Reusable movement behaviours

    private EnemyPatrolSystem patrolSystem;
    private EnemyRetreatSystem retreatSystem;

    // Dialogue

    private EnemyDialogueSystem dialogueSystem;

    private bool isInitialized;

    // General Properties

    public EnemyDefinition Definition => definition;

    public Rigidbody2D EnemyRigidbody => enemyRigidbody;
    public Collider2D MainCollider => mainCollider;
    public Animator Animator => animator;

    public SpriteRenderer[] SpriteRenderers => spriteRenderers;

    public bool IsInitialized => isInitialized;
    public bool IsDead => healthSystem != null && healthSystem.IsDead;
    public bool IsHurting => hurtSystem != null && hurtSystem.IsHurting;

    public int CurrentHealth =>
        healthSystem != null ? healthSystem.CurrentHealth : 0;

    public int MaxHealth =>
        healthSystem != null ? healthSystem.MaxHealth : 0;

    // Movement Properties

    public EnemyStateSystem StateSystem => stateSystem;
    public EnemyMovementSystem MovementSystem => movementSystem;
    public EnemyFacingSystem FacingSystem => facingSystem;
    public EnemyPatrolSystem PatrolSystem => patrolSystem;
    public EnemyRetreatSystem RetreatSystem => retreatSystem;

    public EnemyGroundDetection GroundDetection => groundDetection;
    public EnemyAirStateSystem AirStateSystem => airStateSystem;

    public bool IsJumping =>
        movementSystem != null && movementSystem.IsJumping;

    public bool IsDashing => dashBehaviour != null && dashBehaviour.IsActive;
    public bool IsCharging => chargeBehaviour != null && chargeBehaviour.IsActive;

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

    // Detection and Target Properties

    public EnemyDetectionSystem DetectionSystem => detectionSystem;
    public EnemyTargetScanner TargetScanner => targetScanner;
    public EnemyTargetSystem TargetSystem => targetSystem;

    public IEnemyTargetSearchBehaviour TargetSearchBehaviour =>
        targetSearchBehaviour;

    // Health Properties

    public EnemyHealthSystem HealthSystem => healthSystem;
    public EnemyDamageSystem DamageSystem => damageSystem;
    public EnemyHurtSystem HurtSystem => hurtSystem;
    public EnemyDeathSystem DeathSystem => deathSystem;
    public EnemyFeedbackSystem FeedbackSystem => feedbackSystem;

    // Normal Attack Properties

    public EnemyAttackSystem AttackSystem => attackSystem;

    public EnemyAttackTargetSystem AttackTargetSystem =>
        attackTargetSystem;

    public EnemyAttackDamageSystem AttackDamageSystem =>
        attackDamageSystem;

    public EnemyAttackValidationSystem AttackValidationSystem =>
        attackValidationSystem;

    public EnemyAttackCooldownSystem AttackCooldownSystem =>
        attackCooldownSystem;

    public EnemyAttackStateSystem AttackStateSystem =>
        attackStateSystem;

    public EnemyAttackSequenceSystem AttackSequenceSystem =>
        attackSequenceSystem;

    public EnemyAttackBehaviourSystem AttackBehaviourSystem =>
        attackBehaviourSystem;

    // Special Attack Properties

    public EnemySpecialAttackSystem SpecialAttackSystem =>
        specialAttackSystem;

    public EnemySpecialAttackSequenceSystem SpecialAttackSequenceSystem =>
        specialAttackSequenceSystem;

    public EnemySpecialAttackBehaviourFactory SpecialAttackBehaviourFactory =>
        specialAttackBehaviourFactory;

    // Combined Attack Properties

    public EnemyAttackState CurrentAttackState
    {
        get
        {
            if (specialAttackSystem != null &&
                specialAttackSystem.IsAttackActive &&
                specialAttackSequenceSystem != null)
            {
                return specialAttackSequenceSystem.CurrentState;
            }

            return attackSequenceSystem != null
                ? attackSequenceSystem.CurrentState
                : EnemyAttackState.Idle;
        }
    }

    public bool IsAttackSequenceRunning =>
        (attackSequenceSystem != null &&
         attackSequenceSystem.IsSequenceRunning) ||
        (specialAttackSequenceSystem != null &&
         specialAttackSequenceSystem.IsSequenceRunning);

    public bool IsAttackInProgress =>
        (attackSequenceSystem != null &&
         (attackSequenceSystem.CurrentState == EnemyAttackState.Preparing ||
          attackSequenceSystem.CurrentState == EnemyAttackState.Attacking ||
          attackSequenceSystem.CurrentState == EnemyAttackState.Recovery)) ||
        (specialAttackSequenceSystem != null &&
         specialAttackSequenceSystem.IsSequenceRunning);

    public bool IsAttacking =>
        (attackStateSystem != null &&
         attackStateSystem.IsAttacking) ||
        (specialAttackSystem != null &&
         specialAttackSystem.IsAttackActive &&
         specialAttackSequenceSystem != null &&
         specialAttackSequenceSystem.CurrentState == EnemyAttackState.Attacking);

    public bool IsAttackOnCooldown =>
        attackCooldownSystem != null &&
        attackCooldownSystem.IsOnCooldown;

    public bool HasAnimator => animationController != null &&
                           animationController.IsInitialized;

    // Dialogue

    public EnemyDialogueSystem DialogueSystem => dialogueSystem;

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (!isInitialized || IsDead)
            return;

        groundDetection.CheckGround();
        airStateSystem.Update();
        targetSearchBehaviour?.Update(Time.fixedDeltaTime);
        patrolSystem?.Update(Time.fixedDeltaTime);
        retreatSystem?.Update(Time.fixedDeltaTime);
        dashBehaviour?.TickCooldown(Time.fixedDeltaTime);
        chargeBehaviour?.TickCooldown(Time.fixedDeltaTime);
        bool hadActiveMovementAction = movementSystem.HasActiveAction;
        movementSystem.Update(Time.fixedDeltaTime);

        if (hadActiveMovementAction && !movementSystem.HasActiveAction)
            RefreshAIStateAfterMovement();

        UpdateMovementAnimation();
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        deathSystem?.Update(Time.deltaTime);

        if (IsDead)
            return;

        if (hurtSystem != null && hurtSystem.Update(Time.deltaTime))
            CompleteHurtAnimation();

        if (attackTargetSystem != null && targetSystem != null)
        {
            attackTargetSystem.SetTarget(targetSystem.CurrentTarget);
        }

        attackSequenceSystem?.Update(Time.deltaTime);

        bool specialAttackWasActive =
            specialAttackSystem != null && specialAttackSystem.IsAttackActive;
        specialAttackSequenceSystem?.Update(Time.deltaTime);

        if (specialAttackWasActive &&
            specialAttackSequenceSystem != null &&
            specialAttackSequenceSystem.CurrentState == EnemyAttackState.Idle)
        {
            specialAttackSystem?.CompleteActiveAttack();
        }

        if (!IsDead)
        {
            specialAttackSystem?.Update(
                Time.deltaTime,
                healthSystem.HealthPercentage);
        }

        RefreshAttackState();

        dialogueSystem?.Update(
            Time.deltaTime,
            stateSystem.CurrentState == EnemyState.Idle);
    }

    // Initialization

    public void Initialize()
    {
        if (isInitialized)
            return;

        CleanupRuntimeSystems();

        if (!InitializeSystems())
            CleanupRuntimeSystems();
        else
            isInitialized = true;
    }

    private bool InitializeSystems()
    {

        if (definition == null)
        {
            Debug.LogError(
                $"EnemyDefinition is missing on {gameObject.name}.",
                gameObject);

            return false;
        }

        if (definition.UnsupportedCapabilities != EnemyCapability.None)
        {
            Debug.LogWarning(
                $"Enemy {gameObject.name} has reserved, unsupported capabilities that will be ignored: {definition.UnsupportedCapabilities}.",
                gameObject);
        }

        if ((definition.ConfiguredCapabilities & EnemyCapability.Dashing) != 0 &&
            (definition.DashSettings == null || !definition.DashSettings.Enabled))
        {
            Debug.LogWarning(
                $"Enemy {gameObject.name} has Dashing configured without enabled dash settings; dashing is inactive.",
                gameObject);
        }

        if ((definition.ConfiguredCapabilities & EnemyCapability.Charging) != 0 &&
            (definition.ChargeSettings == null || !definition.ChargeSettings.Enabled))
        {
            Debug.LogWarning(
                $"Enemy {gameObject.name} has Charging configured without enabled charge settings; charging is inactive.",
                gameObject);
        }

        if (definition.DashSettings != null && definition.DashSettings.Enabled &&
            (definition.ConfiguredCapabilities & EnemyCapability.Dashing) == 0)
        {
            Debug.LogWarning(
                $"Enemy {gameObject.name} has enabled dash settings without the Dashing capability; the settings are inactive.",
                gameObject);
        }

        if (definition.ChargeSettings != null && definition.ChargeSettings.Enabled &&
            (definition.ConfiguredCapabilities & EnemyCapability.Charging) == 0)
        {
            Debug.LogWarning(
                $"Enemy {gameObject.name} has enabled charge settings without the Charging capability; the settings are inactive.",
                gameObject);
        }

        CacheComponents();

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                $"Rigidbody2D is missing on {gameObject.name}.",
                gameObject);

            return false;
        }

        if (animator != null)
        {
            animationController = new EnemyAnimationController();
            animationController.Initialize(animator);
        }

        InitializeAnimationEventRelay();

        stateSystem = new EnemyStateSystem();
        movementSystem = new EnemyMovementSystem(this);

        InitializeGroundDetection();
        if (RequiresGroundDetection() && !groundDetection.IsInitialized)
            return false;

        InitializeAirStateSystem();
        if (!airStateSystem.IsInitialized)
            return false;

        if (!InitializeDefaultMovementBehaviour())
            return false;

        if (!InitializeJumpBehaviour())
            return false;

        if (!InitializeDashAndChargeBehaviours())
            return false;
        InitializeFacingSystem();
        if (!facingSystem.IsInitialized)
            return false;

        InitializeDetectionSystem();
        InitializeTargetScanner();
        if (detectionSystem == null || !detectionSystem.IsInitialized ||
            targetScanner == null || !targetScanner.IsInitialized)
        {
            Debug.LogError(
                $"Detection Origin, Visual Root, and a positive scan radius are required on {gameObject.name}.",
                gameObject);
            return false;
        }

        if (targetLayers.value == 0)
        {
            Debug.LogWarning(
                $"Target Layers are empty on {gameObject.name}; this enemy will not acquire targets.",
                gameObject);
        }

        if (definition.DetectionRange <= 0f)
        {
            Debug.LogWarning(
                $"Detection Range is not positive on {gameObject.name}; this enemy will only see targets at its origin.",
                gameObject);
        }

        targetSystem = new EnemyTargetSystem();

        InitializeTargetSearchBehaviour();
        if (targetSearchBehaviour == null || !targetSearchBehaviour.IsInitialized)
            return false;

        InitializePatrolSystem();
        if (patrolSystem == null || !patrolSystem.IsInitialized)
            return false;

        InitializeRetreatSystem();
        if (retreatSystem == null || !retreatSystem.IsInitialized)
            return false;

        if (!InitializeHealthSystems())
            return false;

        if (!InitializeAttackSystems())
            return false;

        InitializeDialogueSystem();

        return true;
    }

    private void CacheComponents()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        enemyColliders = GetComponentsInChildren<Collider2D>(true);
        animator = GetComponentInChildren<Animator>(true);
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    // Movement Initialization

    private void InitializeGroundDetection()
    {
        groundDetection = new EnemyGroundDetection();

        if (!RequiresGroundDetection())
            return;

        groundDetection.Initialize(
            groundCheck,
            groundLayer,
            groundCheckRadius);
    }

    private bool RequiresGroundDetection()
    {
        bool flies = definition != null &&
                     definition.HasCapability(EnemyCapability.Flying);
        bool swims = definition != null &&
                     definition.HasCapability(EnemyCapability.Swimming);
        bool jumps = definition != null &&
                     definition.HasCapability(EnemyCapability.Jumping);

        return (!flies && !swims) || jumps;
    }

    private void InitializeAirStateSystem()
    {
        airStateSystem = new EnemyAirStateSystem();

        airStateSystem.Initialize(
            enemyRigidbody,
            groundDetection);

        airStateSystem.Update();
    }

    private bool InitializeDefaultMovementBehaviour()
    {
        bool canFly = definition.HasCapability(EnemyCapability.Flying);
        bool canSwim = definition.HasCapability(EnemyCapability.Swimming);
        bool canMoveOnGround = definition.HasCapability(EnemyCapability.GroundMovement);

        int movementTypes = (canFly ? 1 : 0) +
                            (canSwim ? 1 : 0) +
                            (canMoveOnGround ? 1 : 0);

        if (movementTypes != 1)
        {
            Debug.LogError(
                movementTypes == 0
                    ? $"Enemy {gameObject.name} has no implemented locomotion capability. Configure exactly one movement mode."
                    : $"Enemy {gameObject.name} has conflicting locomotion capabilities. Configure exactly one movement mode.",
                gameObject);

            return false;
        }

        IEnemyMovementBehaviour behaviour = canFly
            ? (IEnemyMovementBehaviour)new FlyingMovement()
            : canSwim
                ? new SwimmingMovement()
                : new GroundMovement();

        movementSystem.SetBehaviour(behaviour);
        return true;
    }

    private bool InitializeJumpBehaviour()
    {
        if (!definition.HasCapability(EnemyCapability.Jumping))
        {
            jumpBehaviour = null;
            return true;
        }

        jumpBehaviour = new JumpBehaviour();

        jumpBehaviour.Initialize(
            this,
            groundDetection,
            jumpForce,
            takeoffTimeout);

        movementSystem.SetJumpBehaviour(jumpBehaviour);
        if (!jumpBehaviour.IsInitialized)
        {
            Debug.LogError($"Failed to initialize jump behaviour on {gameObject.name}.", gameObject);
            return false;
        }

        return true;
    }

    private bool InitializeDashAndChargeBehaviours()
    {
        dashBehaviour = null;
        chargeBehaviour = null;

        if (definition.HasCapability(EnemyCapability.Dashing))
        {
            EnemyDashSettings settings = definition.DashSettings;
            if (settings == null)
                return FailMovementInitialization("Dash settings are missing.");

            dashBehaviour = new DashBehaviour();
            dashBehaviour.Initialize(
                this,
                settings.Speed,
                settings.Duration,
                settings.Cooldown);

            if (!dashBehaviour.IsInitialized)
                return FailMovementInitialization("Failed to initialize dash behaviour.");
        }

        if (definition.HasCapability(EnemyCapability.Charging))
        {
            EnemyChargeSettings settings = definition.ChargeSettings;
            if (settings == null)
                return FailMovementInitialization("Charge settings are missing.");

            chargeBehaviour = new ChargeBehaviour();
            chargeBehaviour.Initialize(
                this,
                settings.Speed,
                settings.Duration,
                settings.Cooldown);

            if (!chargeBehaviour.IsInitialized)
                return FailMovementInitialization("Failed to initialize charge behaviour.");
        }

        return true;
    }

    private bool FailMovementInitialization(string message)
    {
        Debug.LogError($"{message} Enemy: {gameObject.name}.", gameObject);
        return false;
    }

    private void InitializeFacingSystem()
    {
        facingSystem = new EnemyFacingSystem();
        facingSystem.Initialize(visualRoot);
    }

    // Facing API

    public void Face(EnemyFacingDirection direction)
    {
        if (!isInitialized)
            return;

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
        if (!isInitialized || target == null)
            return;

        Vector2 currentPosition = enemyRigidbody != null
            ? enemyRigidbody.position
            : (Vector2)transform.position;

        facingSystem.FaceTarget(
            target.position,
            currentPosition);
    }

    public bool IsFacing(EnemyFacingDirection direction)
    {
        if (!isInitialized)
            return false;

        return facingSystem.IsFacing(direction);
    }

    // Movement API

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

    public bool Jump()
    {
        if (!isInitialized || IsDead || IsHurting || IsAttackInProgress ||
            definition == null ||
            !definition.HasCapability(EnemyCapability.Jumping))
        {
            return false;
        }

        return movementSystem.Jump();
    }

    public bool TryDash(Vector2 direction)
    {
        if (!isInitialized || IsDead || IsHurting || IsAttackInProgress ||
            definition == null || !definition.HasCapability(EnemyCapability.Dashing) ||
            dashBehaviour == null || !dashBehaviour.IsReady ||
            IsPatrolRetreatOrSearching())
        {
            return false;
        }

        if (float.IsNaN(direction.x) || float.IsInfinity(direction.x) ||
            float.IsNaN(direction.y) || float.IsInfinity(direction.y) ||
            direction.sqrMagnitude <= 0f)
        {
            return false;
        }

        bool started = movementSystem.TryActivateAction(
            dashBehaviour,
            () => dashBehaviour.StartDash(direction));

        if (started)
        {
            if (Mathf.Abs(direction.x) > 0.001f)
                Face(direction.x < 0f ? EnemyFacingDirection.Left : EnemyFacingDirection.Right);

            SetAIState(targetSystem != null && targetSystem.HasTarget
                ? EnemyState.Chase
                : EnemyState.Idle);
        }

        return started;
    }

    public bool TryCharge(Vector2 targetPosition)
    {
        if (!isInitialized || IsDead || IsHurting || IsAttackInProgress ||
            definition == null || !definition.HasCapability(EnemyCapability.Charging) ||
            chargeBehaviour == null || !chargeBehaviour.IsReady ||
            IsPatrolRetreatOrSearching())
        {
            return false;
        }

        if (float.IsNaN(targetPosition.x) || float.IsInfinity(targetPosition.x) ||
            float.IsNaN(targetPosition.y) || float.IsInfinity(targetPosition.y))
        {
            return false;
        }

        Vector2 direction = targetPosition - (Vector2)transform.position;
        bool started = movementSystem.TryActivateAction(
            chargeBehaviour,
            () => chargeBehaviour.StartCharge(targetPosition));

        if (started)
        {
            if (Mathf.Abs(direction.x) > 0.001f)
                Face(direction.x < 0f ? EnemyFacingDirection.Left : EnemyFacingDirection.Right);

            SetAIState(targetSystem != null && targetSystem.HasTarget
                ? EnemyState.Chase
                : EnemyState.Idle);
        }

        return started;
    }

    public bool IsDashReady => dashBehaviour != null && dashBehaviour.IsReady;
    public bool IsChargeReady => chargeBehaviour != null && chargeBehaviour.IsReady;

    private bool IsPatrolRetreatOrSearching()
    {
        return (patrolSystem != null && patrolSystem.IsPatrolling) ||
               (retreatSystem != null && retreatSystem.IsRetreating) ||
               IsTargetSearchBehaviourActive();
    }

    public bool TryActivateMovementAction(
        IEnemyMovementAction action,
        System.Func<bool> startAction)
    {
        if (!isInitialized)
            return false;

        return movementSystem.TryActivateAction(
            action,
            startAction);
    }

    public bool StopMovementAction(IEnemyMovementAction action)
    {
        if (!isInitialized)
            return false;

        return movementSystem.StopAction(action);
    }

    public bool IsMovementActionActive(IEnemyMovementAction action)
    {
        if (!isInitialized)
            return false;

        return movementSystem.IsActionActive(action);
    }

    public void StopActiveMovementAction()
    {
        if (!isInitialized)
            return;

        movementSystem.StopActiveAction();
    }

    public void LockMovement()
    {
        if (!isInitialized)
            return;

        movementSystem.Lock();
    }

    public void UnlockMovement()
    {
        if (!isInitialized)
            return;

        movementSystem.Unlock();
    }

    public bool StartPatrol()
    {
        if (!isInitialized || IsDead || IsHurting || IsAttackInProgress ||
            IsSpecialMovementActive ||
            targetSystem == null || targetSystem.HasTarget)
            return false;

        StopTargetSearching();
        retreatSystem?.StopRetreat();
        return patrolSystem != null && patrolSystem.StartPatrol();
    }

    public void StopPatrol()
    {
        patrolSystem?.StopPatrol();
    }

    public bool TryStartRetreat(Transform target)
    {
        if (!isInitialized || IsDead || IsHurting || IsAttackInProgress ||
            IsSpecialMovementActive || retreatSystem == null)
            return false;

        if (retreatSystem.IsRetreating)
            return true;

        float healthPercentage = healthSystem != null
            ? healthSystem.HealthPercentage
            : 1f;

        if (!retreatSystem.ShouldRetreat(target, healthPercentage))
            return false;

        return StartRetreat(target);
    }

    public bool StartRetreat(Transform target)
    {
        if (!isInitialized || IsDead || IsHurting || IsAttackInProgress ||
            IsSpecialMovementActive || retreatSystem == null)
            return false;

        StopTargetSearching();
        patrolSystem?.StopPatrol();
        return retreatSystem.StartRetreat(target);
    }

    public void StopRetreat()
    {
        retreatSystem?.StopRetreat();
    }

    public void SetAIState(EnemyState state)
    {
        if (!isInitialized || IsDead || stateSystem == null)
            return;

        stateSystem.SetState(state);
    }

    public void RefreshAIStateAfterMovement()
    {
        if (stateSystem == null)
            return;

        if (IsDead)
        {
            stateSystem.SetState(EnemyState.Dead);
            return;
        }

        if (IsHurting)
        {
            stateSystem.SetState(EnemyState.Hurt);
            return;
        }

        if (IsAttackInProgress)
        {
            stateSystem.SetState(EnemyState.Attack);
            return;
        }

        if (retreatSystem != null && retreatSystem.IsRetreating)
        {
            stateSystem.SetState(EnemyState.Retreat);
            return;
        }

        if (patrolSystem != null && patrolSystem.IsPatrolling)
        {
            stateSystem.SetState(EnemyState.Patrol);
            return;
        }

        if (targetSystem != null &&
            (targetSystem.CurrentState == EnemyTargetState.Searching ||
             targetSystem.CurrentState == EnemyTargetState.Lost))
        {
            stateSystem.SetState(EnemyState.Returning);
            return;
        }

        stateSystem.SetState(targetSystem != null && targetSystem.HasTarget
            ? EnemyState.Chase
            : EnemyState.Idle);
    }

    // Detection Initialization

    private void InitializeDetectionSystem()
    {
        detectionSystem = new EnemyDetectionSystem();

        EnemyDetectionSettings settings =
            new EnemyDetectionSettings(
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

    public bool IsTargetInNormalAttackRangeAndSight(Transform target)
    {
        if (!isInitialized || target == null || attackOrigin == null ||
            definition == null || definition.AttackType == EnemyAttackType.None ||
            attackLineOfSightSystem == null || !attackLineOfSightSystem.IsInitialized)
        {
            return false;
        }

        if (Vector2.Distance(attackOrigin.position, target.position) > definition.AttackRange)
            return false;

        return attackLineOfSightSystem.HasLineOfSight(target);
    }

    public Transform FindNearestVisibleTarget()
    {
        if (!isInitialized || targetScanner == null)
            return null;

        return targetScanner.FindNearestVisibleTarget();
    }

    // Target API

    public bool SetTarget(Transform target)
    {
        if (!isInitialized || IsDead || targetSystem == null)
            return false;

        bool targetSet = targetSystem.SetTarget(target);

        if (targetSet)
        {
            patrolSystem?.StopPatrol();
            PlayDialogue(EnemyDialogueType.PlayerDetected);
        }

        return targetSet;
    }

    public bool StartTrackingTarget()
    {
        bool started = targetSystem != null && targetSystem.StartTracking();

        if (started)
            SetAIState(EnemyState.Chase);

        return started;
    }

    public bool ReacquireTarget()
    {
        bool reacquired = targetSystem != null && targetSystem.ReacquireTarget();

        if (reacquired)
            SetAIState(EnemyState.Chase);

        return reacquired;
    }

    public void UpdateLastKnownTargetPosition()
    {
        targetSystem?.UpdateLastKnownPosition();
    }

    public bool LoseTarget()
    {
        if (targetSystem == null)
            return false;

        bool targetLost = targetSystem.LoseTarget();

        if (targetLost)
            PlayDialogue(EnemyDialogueType.LostTarget);

        return targetLost;
    }

    public bool StartSearchingForTarget()
    {
        bool started = targetSystem != null && targetSystem.StartSearching();

        if (started)
            SetAIState(EnemyState.Returning);

        return started;
    }

    public void ClearTarget()
    {
        targetSystem?.ClearTarget();
        retreatSystem?.StopRetreat();
        RefreshAIStateAfterMovement();
    }

    public bool CanCurrentlySeeTarget(Transform target)
    {
        return detectionSystem != null &&
               detectionSystem.CanSeeTarget(target);
    }

    public bool CompleteTargetSearching()
    {
        bool completed = targetSystem != null && targetSystem.CompleteSearching();

        if (completed)
            RefreshAIStateAfterMovement();

        return completed;
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

        if (!targetSearchBehaviour.IsInitialized)
        {
            Debug.LogError($"Failed to initialize target search behaviour on {gameObject.name}.", gameObject);
            targetSearchBehaviour = null;
        }
    }

    public void StartTargetSearching(Vector2 targetPosition)
    {
        patrolSystem?.StopPatrol();
        retreatSystem?.StopRetreat();
        targetSearchBehaviour?.StartSearching(targetPosition);
    }

    public void UpdateTargetSearchPosition(Vector2 targetPosition)
    {
        targetSearchBehaviour?.SetSearchPosition(targetPosition);
    }

    public void StopTargetSearching()
    {
        targetSearchBehaviour?.StopSearching();
        RefreshAIStateAfterMovement();
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

    private void InitializePatrolSystem()
    {
        patrolSystem = new EnemyPatrolSystem();
        patrolSystem.Initialize(this, definition.PatrolSettings);
    }

    private void InitializeRetreatSystem()
    {
        retreatSystem = new EnemyRetreatSystem();
        retreatSystem.Initialize(this, definition.RetreatSettings);
    }

    // Health Initialization

    private bool InitializeHealthSystems()
    {
        healthEventHandler?.Dispose();
        healthEventHandler = null;

        healthSystem = new EnemyHealthSystem(
            definition.MaxHealth,
            definition.Defense);

        if (healthSystem.MaxHealth <= 0)
        {
            Debug.LogError(
                $"Failed to initialize health system on {gameObject.name}.",
                gameObject);

            return false;
        }

        damageSystem = new EnemyDamageSystem();
        damageSystem.Initialize(healthSystem);

        if (!damageSystem.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize damage system on {gameObject.name}.",
                gameObject);

            return false;
        }

        knockbackBehaviour = new KnockbackBehaviour();
        knockbackBehaviour.Initialize(this, knockbackDuration);

        hurtSystem = new EnemyHurtSystem();

        hurtSystem.Initialize(
            this,
            enemyRigidbody,
            knockbackBehaviour,
            knockbackForce,
            verticalKnockbackForce,
            hurtAnimationFallbackDuration);

        if (!hurtSystem.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize hurt system on {gameObject.name}.",
                gameObject);

            return false;
        }

        deathSystem = new EnemyDeathSystem();

        deathSystem.Initialize(
            this,
            enemyRigidbody,
            enemyColliders,
            deathDestroyDelay,
            deathAnimationTimeout);


        if (!deathSystem.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize death system on {gameObject.name}.",
                gameObject);

            return false;
        }

        feedbackSystem = new EnemyFeedbackSystem();

        if (feedbackAudioSource == null)
            feedbackAudioSource = GetComponent<AudioSource>();

        feedbackSystem.Initialize(
            feedbackAudioSource,
            hurtSound,
            deathSound,
            healSound,
            hurtEffect,
            deathEffect,
            healEffect);

        if (!feedbackSystem.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize feedback system on {gameObject.name}.",
                gameObject);

            return false;
        }

        healthEventHandler = new EnemyHealthEventHandler();

        healthEventHandler.Initialize(
            healthSystem,
            damageSystem,
            hurtSystem,
            deathSystem,
            feedbackSystem,
            dialogueType =>
            {
                PlayDialogue(dialogueType);
            });

        if (!healthEventHandler.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize health event handler on {gameObject.name}.",
                gameObject);

            healthEventHandler.Dispose();
            healthEventHandler = null;

            return false;
        }

        return true;
    }

    // Attack Initialization

    private bool InitializeAttackSystems()
    {
        ClearAttackSystems();

        bool hasNormalAttack =
            definition.AttackType != EnemyAttackType.None;

        bool hasSpecialAttacks =
            definition.SpecialAttacks != null &&
            System.Array.Exists(
                definition.SpecialAttacks,
                item => item != null && item.AttackType != EnemySpecialAttackType.None);

        if (!hasNormalAttack && !hasSpecialAttacks)
            return true;

        if (animator == null)
        {
            return FailAttackInitialization(
                "Animator is missing. Attack systems cannot be initialized.");
        }

        if (hasNormalAttack)
        {
            if (!InitializeNormalAttackSystems())
                return false;
        }

        if (hasSpecialAttacks)
        {
            if (!InitializeSpecialAttackSystems())
                return false;
        }

        if (!InitializeAttackAnimationEventHandlers())
        {
            return FailAttackInitialization(
                "Failed to initialize attack animation event handlers.");
        }

        return true;
    }

    private bool InitializeNormalAttackSystems()
    {
        switch (definition.AttackType)
        {
            case EnemyAttackType.Melee:
            case EnemyAttackType.Ranged:
            case EnemyAttackType.Area:
                break;

            default:
                return FailAttackInitialization(
                    $"Unsupported attack type: {definition.AttackType}.");
        }

        if (attackOrigin == null)
        {
            return FailAttackInitialization(
                "Attack Origin is missing.");
        }

        attackSystem = new EnemyAttackSystem();
        attackSystem.Initialize(this);

        if (!attackSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack system.");
        }

        attackTargetSystem = new EnemyAttackTargetSystem();

        attackTargetSystem.Initialize(
            attackOrigin,
            definition.AttackRange);

        if (!attackTargetSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack target system.");
        }

        attackDamageSystem = new EnemyAttackDamageSystem();
        attackDamageSystem.Initialize(definition.AttackDamage);

        if (!attackDamageSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack damage system.");
        }

        attackLineOfSightSystem = new EnemyAttackLineOfSightSystem();

        attackLineOfSightSystem.Initialize(
            attackOrigin,
            obstacleLayers);

        if (!attackLineOfSightSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack line of sight system.");
        }

        attackValidationSystem = new EnemyAttackValidationSystem();

        attackValidationSystem.Initialize(
            attackSystem,
            attackTargetSystem,
            attackLineOfSightSystem);

        if (!attackValidationSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack validation system.");
        }

        if (definition.AttackType == EnemyAttackType.Melee)
        {
            meleeHitDetectionSystem =
                new EnemyMeleeHitDetectionSystem();

            meleeHitDetectionSystem.Initialize(
                attackOrigin,
                definition.AttackRange,
                targetLayers);

            if (!meleeHitDetectionSystem.IsInitialized)
            {
                return FailAttackInitialization(
                    "Failed to initialize melee hit detection system.");
            }
        }

        attackCooldownSystem = new EnemyAttackCooldownSystem();
        attackCooldownSystem.Initialize(definition.AttackCooldown);

        if (!attackCooldownSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack cooldown system.");
        }

        attackStateSystem = new EnemyAttackStateSystem();

        attackTimingSystem = new EnemyAttackTimingSystem();

        attackTimingSystem.Initialize(
            definition.AttackPreparationDuration,
            definition.AttackDuration,
            definition.AttackRecoveryDuration);

        if (!attackTimingSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack timing system.");
        }

        attackSequenceSystem = new EnemyAttackSequenceSystem();

        attackSequenceSystem.Initialize(
            attackStateSystem,
            attackTimingSystem,
            attackCooldownSystem,
            true,
            attackAnimationEventFallbackGraceDuration);

        if (!attackSequenceSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize attack sequence system.");
        }

        attackBehaviourFactory = new EnemyAttackBehaviourFactory();

        IEnemyAttackBehaviour behaviour =
            attackBehaviourFactory.Create(
                definition.AttackType,
                attackSystem,
                attackTargetSystem,
                attackDamageSystem,
                attackValidationSystem,
                meleeHitDetectionSystem,
                attackOrigin,
                definition.AttackRange,
                targetLayers,
                projectileSpawnPoint,
                projectilePrefab,
                definition.ProjectileSpeed,
                definition.ProjectileLifetime);

        if (behaviour == null)
        {
            return FailAttackInitialization(
                "Failed to create attack behaviour.");
        }

        attackBehaviourSystem = new EnemyAttackBehaviourSystem();

        if (!attackBehaviourSystem.SetBehaviour(behaviour))
        {
            return FailAttackInitialization(
                "Failed to initialize attack behaviour system.");
        }

        return true;
    }

    private bool InitializeSpecialAttackSystems()
    {
        bool hasRangedSpecialAttack = false;

        foreach (EnemySpecialAttackDefinition specialDefinition
                 in definition.SpecialAttacks)
        {
            if (specialDefinition == null)
                continue;

            if (specialDefinition.AttackType ==
                EnemySpecialAttackType.Ranged)
            {
                hasRangedSpecialAttack = true;
                break;
            }
        }

        // Ranged special attacks need the current target.
        if (hasRangedSpecialAttack && attackTargetSystem == null)
        {
            if (attackOrigin != null)
            {
                attackTargetSystem = new EnemyAttackTargetSystem();

                attackTargetSystem.Initialize(
                    attackOrigin,
                    definition.AttackRange);

                if (!attackTargetSystem.IsInitialized)
                {
                    return FailAttackInitialization(
                        "Failed to initialize target system for ranged special attacks.");
                }
            }
            else
            {
                Debug.LogWarning(
                    $"Attack Origin is missing on {gameObject.name}. Ranged special attacks cannot execute.",
                    gameObject);
            }
        }

        specialAttackStateSystem = new EnemyAttackStateSystem();

        specialAttackSequenceSystem =
            new EnemySpecialAttackSequenceSystem();

        specialAttackSequenceSystem.Initialize(
            specialAttackStateSystem,
            attackAnimationEventFallbackGraceDuration);

        if (!specialAttackSequenceSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize special attack sequence system.");
        }

        specialAttackBehaviourFactory =
            new EnemySpecialAttackBehaviourFactory(
                new EnemySpecialAttackMeleeBehaviour(
                    attackOrigin,
                    targetLayers),

                new EnemySpecialAttackRangedBehaviour(
                    attackTargetSystem,
                    projectileSpawnPoint),

                new EnemySpecialAttackAreaBehaviour(
                    attackOrigin,
                    targetLayers));

        specialAttackSystem = new EnemySpecialAttackSystem(
            definition.SpecialAttacks,
            definition.SpecialAttackCheckIntervalMin,
            definition.SpecialAttackCheckIntervalMax,
            definition.SpecialAttackHealthThreshold,
            TryStartSpecialAttack);

        if (!specialAttackSystem.IsInitialized)
        {
            return FailAttackInitialization(
                "Failed to initialize special attack system.");
        }

        return true;
    }

    // Special Attack Start

    private bool TryStartSpecialAttack(
        EnemySpecialAttackDefinition specialDefinition)
    {
        if (IsDead || IsHurting || IsPatrolRetreatOrSearching() ||
            IsSpecialMovementActive ||
            specialDefinition == null ||
            targetSystem == null || !targetSystem.HasTarget ||
            specialAttackSystem == null ||
            specialAttackSequenceSystem == null ||
            specialAttackBehaviourFactory == null)
        {
            return false;
        }

        if (specialAttackSystem.IsAttackActive)
            return false;

        // A running normal attack must finish first.
        if (IsAttackInProgress)
        {
            return false;
        }

        if (specialAttackSequenceSystem.IsSequenceRunning)
            return false;

        if (!specialAttackBehaviourFactory.CanExecute(specialDefinition))
            return false;

        if (!specialAttackSequenceSystem.StartSequence(specialDefinition))
            return false;

        StopNormalMovement();
        SetAIState(EnemyState.Attack);
        animationController?.SetAttack();

        PlayDialogue(EnemyDialogueType.SpecialAttack);

        return true;
    }

    // Normal Attack Start

    public bool TryStartAttack()
    {
        if (!isInitialized || IsDead || IsHurting ||
            IsPatrolRetreatOrSearching() || IsSpecialMovementActive)
            return false;

        // An active special attack blocks normal attacks.
        if (specialAttackSystem != null &&
            specialAttackSystem.IsAttackActive)
        {
            return false;
        }

        // A pending special attack has priority.
        if (specialAttackSystem != null &&
            specialAttackSystem.IsAttackPending)
        {
            specialAttackSystem.TryStartPendingAttack();

            if (specialAttackSystem.IsAttackActive)
            {
                return false;
            }
        }

        if (attackSequenceSystem == null ||
            attackValidationSystem == null ||
            attackBehaviourSystem == null)
        {
            return false;
        }

        if (attackTargetSystem != null && targetSystem != null)
            attackTargetSystem.SetTarget(targetSystem.CurrentTarget);

        if (!attackValidationSystem.CanAttack())
            return false;

        bool attackStarted = attackSequenceSystem.StartSequence();

        if (attackStarted)
        {
            StopNormalMovement();
            SetAIState(EnemyState.Attack);
            animationController?.SetAttack();
            PlayDialogue(EnemyDialogueType.Attack);
        }

        return attackStarted;
    }

    // Attack Event Handlers

    private bool InitializeAttackAnimationEventHandlers()
    {
        if (animator == null)
        {
            Debug.LogError(
                $"Animator is missing on {gameObject.name}. Attack animation events cannot be initialized.",
                gameObject);

            return false;
        }

        attackAnimationEventHandler =
            animator.GetComponent<EnemyAttackAnimationEventHandler>();

        if (attackAnimationEventHandler == null)
        {
            attackAnimationEventHandler =
                animator.gameObject.AddComponent<EnemyAttackAnimationEventHandler>();
        }

        attackAnimationEventHandler.Initialize(
            attackSequenceSystem,
            specialAttackSequenceSystem,
            specialAttackSystem);

        if (!attackAnimationEventHandler.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize attack animation event handler on {gameObject.name}.",
                gameObject);

            attackAnimationEventHandler.Clear();
            attackAnimationEventHandler = null;

            return false;
        }

        attackHitEventHandler =
            animator.GetComponent<EnemyAttackHitEventHandler>();

        if (attackHitEventHandler == null)
        {
            attackHitEventHandler =
                animator.gameObject.AddComponent<EnemyAttackHitEventHandler>();
        }

        attackHitEventHandler.Initialize(
            attackBehaviourSystem,
            attackStateSystem,
            attackSystem,
            specialAttackSystem,
            specialAttackSequenceSystem,
            specialAttackBehaviourFactory);

        if (!attackHitEventHandler.IsInitialized)
        {
            Debug.LogError(
                $"Failed to initialize attack hit event handler on {gameObject.name}.",
                gameObject);

            attackAnimationEventHandler.Clear();
            attackHitEventHandler.Clear();

            attackAnimationEventHandler = null;
            attackHitEventHandler = null;

            return false;
        }

        return true;
    }

    private bool FailAttackInitialization(string message)
    {
        Debug.LogError(
            $"{message} Enemy: {gameObject.name}.",
            gameObject);

        ClearAttackSystems();

        return false;
    }

    private void ClearAttackSystems()
    {
        attackAnimationEventHandler?.Clear();
        attackHitEventHandler?.Clear();

        attackSystem = null;
        attackTargetSystem = null;
        attackDamageSystem = null;
        attackLineOfSightSystem = null;
        attackValidationSystem = null;

        meleeHitDetectionSystem = null;

        attackCooldownSystem = null;
        attackStateSystem = null;
        attackTimingSystem = null;
        attackSequenceSystem = null;

        attackBehaviourSystem?.ClearBehaviour();

        attackBehaviourSystem = null;
        attackBehaviourFactory = null;

        specialAttackSystem?.Clear();
        specialAttackSequenceSystem?.Clear();

        specialAttackSystem = null;
        specialAttackStateSystem = null;
        specialAttackSequenceSystem = null;
        specialAttackBehaviourFactory = null;

        attackAnimationEventHandler = null;
        attackHitEventHandler = null;
    }

    // Health API


    public int ReceiveDamage(
        int incomingDamage,
        Vector2 attackPosition)
    {
        if (!isInitialized || IsDead)
            return 0;

        int appliedDamage = damageSystem.ReceiveDamage(
            incomingDamage,
            attackPosition);

        if (appliedDamage <= 0)
            return 0;

        attackSequenceSystem?.Interrupt();

        specialAttackSequenceSystem?.Interrupt();
        specialAttackSystem?.InterruptActiveAttack();

        if (IsDead)
        {
            stateSystem?.SetState(EnemyState.Dead);
            return appliedDamage;
        }

        SetAIState(EnemyState.Hurt);

        if (IsHurting && HasAnimator)
        {
            animationController?.SetHurt();
        }
        else if (IsHurting)
        {
            CompleteHurtAnimation();
        }

        return appliedDamage;
    }


    public int Heal(int healAmount)
    {
        if (!isInitialized || IsDead)
            return 0;

        return healthSystem.Heal(healAmount);
    }

    public void CompleteHurtAnimation()
    {
        if (!IsHurting)
            return;

        hurtSystem?.CompleteHurt();

        if (IsDead)
            return;

        attackSequenceSystem?.Reset();
        specialAttackSequenceSystem?.Reset();
        specialAttackSystem?.InterruptActiveAttack();

        RefreshAIStateAfterMovement();
        UpdateMovementAnimation();
    }

    public void OnDeathStarted()
    {
        stateSystem?.SetState(EnemyState.Dead);

        targetSearchBehaviour?.StopSearching();
        targetSystem?.ClearTarget();
        patrolSystem?.StopPatrol();
        retreatSystem?.StopRetreat();

        attackSequenceSystem?.Interrupt();
        specialAttackSequenceSystem?.Interrupt();
        specialAttackSystem?.InterruptActiveAttack();

        movementSystem?.Lock();
    }

    public void CompleteDeathAnimation()
    {
        deathSystem?.CompleteDeathAnimation();
    }

    // Dialogue API

    public bool PlayDialogue(EnemyDialogueType dialogueType)
    {
        if (!isInitialized || dialogueSystem == null)
            return false;

        return dialogueSystem.PlayDialogue(dialogueType);
    }

    private void InitializeDialogueSystem()
    {
        if (dialogueAudioSource == null)
        {
            Debug.LogWarning(
                $"Dialogue AudioSource is missing on {gameObject.name}. Dialogue system will be disabled.",
                gameObject);

            return;
        }

        if (definition.DialogueDefinition == null)
        {
            Debug.LogWarning(
                $"Dialogue Definition is missing on {gameObject.name}. Dialogue system will be disabled.",
                gameObject);

            return;
        }

        if (dialogueAudioSource == feedbackAudioSource)
        {
            Debug.LogError(
                $"Dialogue AudioSource and Feedback AudioSource must be different on {gameObject.name}. Dialogue system will be disabled.",
                gameObject);

            return;
        }

        dialogueSystem = new EnemyDialogueSystem();

        dialogueSystem.Initialize(
            dialogueAudioSource,
            definition.DialogueDefinition);

        if (!dialogueSystem.IsInitialized)
        {
            Debug.LogWarning(
                $"Failed to initialize dialogue system on {gameObject.name}. Dialogue system will be disabled.",
                gameObject);

            dialogueSystem.Clear();
            dialogueSystem = null;
        }
    }

    // Animation Event Relay

    private void InitializeAnimationEventRelay()
    {
        if (animator == null)
            return;

        animationEventRelay =
            animator.GetComponent<EnemyAnimationEventRelay>();

        if (animationEventRelay == null)
        {
            animationEventRelay =
                animator.gameObject.AddComponent<EnemyAnimationEventRelay>();
        }

        animationEventRelay.Initialize(this);
    }

    // Cleanup

    private void OnDestroy()
    {
        CleanupRuntimeSystems();
    }

    private void CleanupRuntimeSystems()
    {
        isInitialized = false;

        healthEventHandler?.Dispose();
        healthEventHandler = null;

        targetSearchBehaviour?.StopSearching();
        patrolSystem?.StopPatrol();
        retreatSystem?.StopRetreat();

        movementSystem?.Reset();
        ClearAttackSystems();

        animationEventRelay?.Clear();
        animationEventRelay = null;

        animationController?.ResetAnimationStates();
        animationController?.Clear();
        animationController = null;

        dialogueSystem?.Clear();
        dialogueSystem = null;

        stateSystem = null;
        movementSystem = null;
        facingSystem = null;
        groundDetection = null;
        airStateSystem = null;
        jumpBehaviour = null;
        dashBehaviour = null;
        chargeBehaviour = null;
        targetScanner = null;
        detectionSystem = null;
        targetSystem = null;
        targetSearchBehaviour = null;
        healthSystem = null;
        damageSystem = null;
        hurtSystem = null;
        deathSystem = null;
        feedbackSystem = null;
        knockbackBehaviour = null;
        patrolSystem = null;
        retreatSystem = null;

        enemyRigidbody = null;
        mainCollider = null;
        enemyColliders = null;
        animator = null;
        spriteRenderers = null;
    }

    private void UpdateMovementAnimation()
    {
        if (IsDead)
            return;

        if (hurtSystem != null && hurtSystem.IsHurting)
            return;

        if (IsAttackInProgress)
            return;

        if (movementSystem.IsMoving)
            animationController?.SetRun();
        else
            animationController?.SetIdle();
    }

    public void PlayDeathAnimation()
    {
        animationController?.SetDeath();
    }

    private void RefreshAttackState()
    {
        if (stateSystem == null || IsDead || IsHurting)
            return;

        if (attackSequenceSystem != null &&
            attackSequenceSystem.CurrentState == EnemyAttackState.Interrupted)
        {
            attackSequenceSystem.Reset();
        }

        if (specialAttackSequenceSystem != null &&
            specialAttackSequenceSystem.CurrentState == EnemyAttackState.Interrupted)
        {
            specialAttackSequenceSystem.Reset();
            specialAttackSystem?.InterruptActiveAttack();
        }

        if (stateSystem.CurrentState != EnemyState.Attack || IsAttackInProgress)
            return;

        RefreshAIStateAfterMovement();
    }

    private void OnValidate()
    {
        groundCheckRadius = ClampFinite(groundCheckRadius, 0f, 0f);
        targetScanRadius = ClampFinite(targetScanRadius, 0f, 0f);
        knockbackForce = ClampFinite(knockbackForce, 0f, 0f);
        verticalKnockbackForce = ClampFinite(verticalKnockbackForce, 0f, 0f);
        knockbackDuration = ClampFinite(knockbackDuration, 0.2f, 0.01f);
        hurtAnimationFallbackDuration =
            ClampFinite(hurtAnimationFallbackDuration, 1f, 0.01f);
        deathDestroyDelay = ClampFinite(deathDestroyDelay, 0f, 0f);
        deathAnimationTimeout = ClampFinite(deathAnimationTimeout, 5f, 0.01f);
        attackAnimationEventFallbackGraceDuration =
            ClampFinite(attackAnimationEventFallbackGraceDuration, 0.5f, 0f);
    }

    private static float ClampFinite(float value, float fallback, float minimum)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            return fallback;

        return Mathf.Max(minimum, value);
    }

}
