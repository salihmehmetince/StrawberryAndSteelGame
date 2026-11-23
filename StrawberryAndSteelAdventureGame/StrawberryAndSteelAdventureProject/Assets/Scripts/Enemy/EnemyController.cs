
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

    [Header("Death Configuration")]
    [SerializeField] private float deathDestroyDelay;

    [Header("Feedback Configuration")]
    [SerializeField] private AudioSource feedbackAudioSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private ParticleSystem hurtEffect;
    [SerializeField] private ParticleSystem deathEffect;

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

    public int CurrentHealth =>
        healthSystem != null ? healthSystem.CurrentHealth : 0;

    public int MaxHealth =>
        healthSystem != null ? healthSystem.MaxHealth : 0;

    // Movement Properties

    public EnemyStateSystem StateSystem => stateSystem;
    public EnemyMovementSystem MovementSystem => movementSystem;
    public EnemyFacingSystem FacingSystem => facingSystem;

    public EnemyGroundDetection GroundDetection => groundDetection;
    public EnemyAirStateSystem AirStateSystem => airStateSystem;

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
        if (!isInitialized)
            return;

        groundDetection.CheckGround();
        airStateSystem.Update();
        movementSystem.Update(Time.fixedDeltaTime);

        UpdateMovementAnimation();

        targetSearchBehaviour?.Update(Time.fixedDeltaTime);
    }
    private void Update()
    {
        if (!isInitialized)
            return;

        if (attackTargetSystem != null && targetSystem != null)
        {
            attackTargetSystem.SetTarget(targetSystem.CurrentTarget);
        }

        attackSequenceSystem?.Update(Time.deltaTime);

        if (!IsDead)
        {
            specialAttackSystem?.Update(
                Time.deltaTime,
                healthSystem.HealthPercentage);
        }

        deathSystem?.Update(Time.deltaTime);

        dialogueSystem?.Update(
            Time.deltaTime,
            stateSystem.CurrentState == EnemyState.Idle);
    }

    // Initialization

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

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                $"Rigidbody2D is missing on {gameObject.name}.",
                gameObject);

            return;
        }

        animationController = new EnemyAnimationController();
        animationController.Initialize(animator);

        InitializeAnimationEventRelay();

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

        if (!InitializeHealthSystems())
            return;

        if (!InitializeAttackSystems())
        {
            healthEventHandler?.Dispose();
            healthEventHandler = null;

            return;
        }

        InitializeDialogueSystem();

        isInitialized = true;
    }

    private void CacheComponents()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>(true);
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    // Movement Initialization

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
        if (!isInitialized)
            return false;

        return movementSystem.Jump();
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

    public Transform FindNearestVisibleTarget()
    {
        if (!isInitialized || targetScanner == null)
            return null;

        return targetScanner.FindNearestVisibleTarget();
    }

    // Target API

    public bool SetTarget(Transform target)
    {
        if (targetSystem == null)
            return false;

        bool targetSet = targetSystem.SetTarget(target);

        if (targetSet)
            PlayDialogue(EnemyDialogueType.PlayerDetected);

        return targetSet;
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
        if (targetSystem == null)
            return false;

        bool targetLost = targetSystem.LoseTarget();

        if (targetLost)
            PlayDialogue(EnemyDialogueType.LostTarget);

        return targetLost;
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

        hurtSystem = new EnemyHurtSystem();

        hurtSystem.Initialize(
            this,
            enemyRigidbody,
            animator,
            knockbackForce,
            verticalKnockbackForce);

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
            mainCollider,
            deathDestroyDelay);


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
            hurtEffect,
            deathEffect);

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
            definition.SpecialAttacks.Length > 0;

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
            true);

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
            specialAttackStateSystem);

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
        if (IsDead ||
            specialDefinition == null ||
            specialAttackSystem == null ||
            specialAttackSequenceSystem == null ||
            specialAttackBehaviourFactory == null)
        {
            return false;
        }

        if (specialAttackSystem.IsAttackActive)
            return false;

        // A running normal attack must finish first.
        if (attackSequenceSystem != null &&
            attackSequenceSystem.IsSequenceRunning)
        {
            return false;
        }

        if (specialAttackSequenceSystem.IsSequenceRunning)
            return false;

        if (!specialAttackBehaviourFactory.CanExecute(specialDefinition))
            return false;

        if (!specialAttackSequenceSystem.StartSequence())
            return false;

        animationController?.SetAttack();

        PlayDialogue(EnemyDialogueType.Attack);

        return true;
    }

    // Normal Attack Start

    public bool TryStartAttack()
    {
        if (!isInitialized || IsDead)
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

            // Keep normal attacks blocked while the request is pending.
            if (specialAttackSystem.IsAttackPending ||
                specialAttackSystem.IsAttackActive)
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

        if (!attackValidationSystem.CanAttack())
            return false;

        bool attackStarted = attackSequenceSystem.StartSequence();

        if (attackStarted)
        {
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
            animationController?.SetDeath();
            return appliedDamage;
        }

        if (hurtSystem != null && hurtSystem.IsHurting)
        {
            animationController?.SetHurt();
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
        hurtSystem?.CompleteHurt();

        if (IsDead)
            return;

        UpdateMovementAnimation();
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
        healthEventHandler?.Dispose();
        healthEventHandler = null;

        ClearAttackSystems();

        animationController?.Clear();
        animationController = null;

        dialogueSystem?.Clear();
        dialogueSystem = null;
    }

    private void UpdateMovementAnimation()
    {
        if (IsDead)
            return;

        if (hurtSystem != null && hurtSystem.IsHurting)
            return;

        if (attackSequenceSystem != null &&
            attackSequenceSystem.IsSequenceRunning)
            return;

        if (specialAttackSequenceSystem != null &&
            specialAttackSequenceSystem.IsSequenceRunning)
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

}
