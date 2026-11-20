
using UnityEngine;

public class EnemyController : MonoBehaviour,IDamageable
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

    [Header("Attack Systems")]
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

    private EnemyAttackAnimationEventHandler attackAnimationEventHandler;
    private EnemyAttackHitEventHandler attackHitEventHandler;

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


    private EnemyHealthSystem healthSystem;
    private EnemyDamageSystem damageSystem;
    private EnemyHurtSystem hurtSystem;
    private EnemyDeathSystem deathSystem;
    private EnemyFeedbackSystem feedbackSystem;
    private EnemyHealthEventHandler healthEventHandler;

    public EnemyDefinition Definition => definition;

    public Rigidbody2D EnemyRigidbody => enemyRigidbody;
    public Collider2D MainCollider => mainCollider;
    public Animator Animator => animator;

    private EnemyAnimationEventRelay animationEventRelay;
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


    public EnemyHealthSystem HealthSystem => healthSystem;
    public EnemyDamageSystem DamageSystem => damageSystem;
    public EnemyHurtSystem HurtSystem => hurtSystem;
    public EnemyDeathSystem DeathSystem => deathSystem;
    public EnemyFeedbackSystem FeedbackSystem => feedbackSystem;

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

    public bool IsDead =>
        healthSystem != null && healthSystem.IsDead;

    public int CurrentHealth =>
        healthSystem != null ? healthSystem.CurrentHealth : 0;

    public int MaxHealth =>
        healthSystem != null ? healthSystem.MaxHealth : 0;

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

    public EnemyAttackState CurrentAttackState =>
    attackSequenceSystem != null
        ? attackSequenceSystem.CurrentState
        : EnemyAttackState.Idle;

    public bool IsAttackSequenceRunning =>
        attackSequenceSystem != null &&
        attackSequenceSystem.IsSequenceRunning;

    public bool IsAttacking =>
        attackStateSystem != null &&
        attackStateSystem.IsAttacking;

    public bool IsAttackOnCooldown =>
        attackCooldownSystem != null &&
        attackCooldownSystem.IsOnCooldown;

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

    private void Update()
    {
        if (!isInitialized)
            return;

        if (attackTargetSystem != null && targetSystem != null)
        {
            attackTargetSystem.SetTarget(targetSystem.CurrentTarget);
        }

        attackSequenceSystem?.Update(Time.deltaTime);

        deathSystem?.Update(Time.deltaTime);
    }


    public void CompleteHurtAnimation()
    {
        hurtSystem?.CompleteHurt();
    }

    public void CompleteDeathAnimation()
    {
        deathSystem?.CompleteDeathAnimation();
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

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                $"Rigidbody2D is missing on {gameObject.name}.",
                gameObject);

            return;
        }

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
            animator,
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
            feedbackSystem);

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



    private bool InitializeAttackSystems()
    {
        ClearAttackSystems();

        switch (definition.AttackType)
        {
            case EnemyAttackType.None:
                return true;

            case EnemyAttackType.Melee:
            case EnemyAttackType.Ranged:
            case EnemyAttackType.Area:
                break;

            default:
                return FailAttackInitialization(
                    $"Unsupported attack type: {definition.AttackType}.");
        }

        if (animator == null)
        {
            return FailAttackInitialization(
                "Animator is missing. Attack systems cannot be initialized.");
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
            meleeHitDetectionSystem = new EnemyMeleeHitDetectionSystem();
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

        if (!InitializeAttackAnimationEventHandlers())
        {
            return FailAttackInitialization(
                "Failed to initialize attack animation event handlers.");
        }

        return true;
    }
    public bool TryStartAttack()
    {
        if (!isInitialized ||
            attackSequenceSystem == null ||
            attackValidationSystem == null ||
            attackBehaviourSystem == null)
        {
            return false;
        }

        if (!attackValidationSystem.CanAttack())
            return false;

        return attackSequenceSystem.StartSequence();
    }
    public int ReceiveDamage(
        int incomingDamage,
        Vector2 attackPosition)
    {
        if (!isInitialized || IsDead)
            return 0;

        int appliedDamage = damageSystem.ReceiveDamage(
            incomingDamage,
            attackPosition);

        if (appliedDamage > 0)
            attackSequenceSystem?.Interrupt();

        return appliedDamage;
    }

    public int Heal(int healAmount)
    {
        if (!isInitialized || IsDead)
            return 0;

        return healthSystem.Heal(healAmount);
    }


    private void OnDestroy()
    {
        healthEventHandler?.Dispose();
        healthEventHandler = null;
    }

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

        attackAnimationEventHandler.Initialize(attackSequenceSystem);

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
            attackSystem);

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

        attackAnimationEventHandler = null;
        attackHitEventHandler = null;
    }
}
