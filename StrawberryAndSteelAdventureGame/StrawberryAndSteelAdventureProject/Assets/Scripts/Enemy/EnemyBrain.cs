using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    protected EnemyController enemyController;

    [Header("Target Scanning")]
    [SerializeField, Min(0.01f)] private float targetScanInterval = 0.25f;

    private float targetScanTimer;
    private float targetSearchTimer;

    public bool IsInitialized { get; private set; }

    public Transform CurrentTarget => enemyController != null
        ? enemyController.TargetSystem?.CurrentTarget
        : null;

    public bool HasTarget => IsTargetValid();

    public Vector2 LastKnownTargetPosition =>
        enemyController != null && enemyController.TargetSystem != null
            ? enemyController.TargetSystem.LastKnownPosition
            : Vector2.zero;

    public bool HasLastKnownTargetPosition =>
        enemyController != null && enemyController.TargetSystem != null &&
        enemyController.TargetSystem.HasLastKnownPosition;

    public EnemyTargetState CurrentTargetState =>
        enemyController != null && enemyController.TargetSystem != null
            ? enemyController.TargetSystem.CurrentState
            : EnemyTargetState.NoTarget;

    protected virtual void Start()
    {
        enemyController = GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError($"{GetType().Name} requires an EnemyController.", this);
            enabled = false;
            return;
        }

        if (!enemyController.IsInitialized)
        {
            Debug.LogError($"{GetType().Name}: EnemyController is not initialized.", this);
            enabled = false;
            return;
        }

        targetScanInterval = IsFinite(targetScanInterval)
            ? Mathf.Max(0.01f, targetScanInterval)
            : 0.25f;
        targetScanTimer = 0f;

        InitializeBrain();
        IsInitialized = true;
    }

    protected virtual void Update()
    {
        if (!IsInitialized || enemyController == null || enemyController.IsDead)
            return;

        float deltaTime = Time.deltaTime;
        UpdateTargetScanning(deltaTime);
        UpdateTargetSearching(deltaTime);

        if (enemyController.IsTargetSearchBehaviourActive() || enemyController.IsHurting ||
            enemyController.IsAttackInProgress)
        {
            return;
        }

        if (enemyController.TryStartRetreat(CurrentTarget))
            return;

        if (enemyController.RetreatSystem != null && enemyController.RetreatSystem.IsRetreating)
            return;

        if (!HasTarget && enemyController.PatrolSystem != null && enemyController.PatrolSystem.CanPatrol)
        {
            if (!enemyController.PatrolSystem.IsPatrolling)
                enemyController.StartPatrol();

            return;
        }

        if (enemyController.PatrolSystem != null && enemyController.PatrolSystem.IsPatrolling)
            return;

        if (TryHandleVariantDecision(deltaTime))
            return;

        if (enemyController.IsDead || enemyController.IsHurting ||
            enemyController.IsAttackInProgress || enemyController.IsSpecialMovementActive)
        {
            Think(deltaTime);
            return;
        }

        if (!HasTarget)
        {
            enemyController.StopNormalMovement();
            enemyController.SetAIState(EnemyState.Idle);
            Think(deltaTime);
            return;
        }

        Transform target = CurrentTarget;
        if (target == null)
        {
            enemyController.StopNormalMovement();
            enemyController.SetAIState(EnemyState.Idle);
            Think(deltaTime);
            return;
        }

        enemyController.FaceTarget(target);

        if (enemyController.TryStartAttack() || enemyController.IsAttackInProgress)
        {
            Think(deltaTime);
            return;
        }

        if (enemyController.IsTargetInNormalAttackRangeAndSight(target))
        {
            enemyController.StopNormalMovement();
            enemyController.SetAIState(EnemyState.Chase);
            Think(deltaTime);
            return;
        }

        float distance = GetDistanceToTarget();
        EnemyDefinition definition = enemyController.Definition;

        if (definition.HasCapability(EnemyCapability.Dashing) &&
            definition.DashSettings != null &&
            definition.DashSettings.ShouldTrigger(distance) &&
            enemyController.TryDash(GetTargetMovementDirection(target)))
        {
            Think(deltaTime);
            return;
        }

        if (definition.HasCapability(EnemyCapability.Charging) &&
            definition.ChargeSettings != null &&
            definition.ChargeSettings.ShouldTrigger(distance) &&
            enemyController.TryCharge(GetChargeTargetPosition(target)))
        {
            Think(deltaTime);
            return;
        }

        ChaseTarget(target);
        Think(deltaTime);
    }

    protected virtual void InitializeBrain()
    {
    }

    /// <summary>Return true when a variant action consumes this decision tick.</summary>
    protected virtual bool TryHandleVariantDecision(float deltaTime)
    {
        return false;
    }

    /// <summary>Runs after the shared decision to allow variant-specific adjustments.</summary>
    protected virtual void Think(float deltaTime)
    {
    }

    private void UpdateTargetScanning(float deltaTime)
    {
        targetScanTimer -= deltaTime;
        if (targetScanTimer > 0f)
            return;

        targetScanTimer = targetScanInterval;

        if (!IsTargetValid())
        {
            if (enemyController.IsTargetSearchBehaviourActive())
                enemyController.StopTargetSearching();

            targetSearchTimer = 0f;
            enemyController.ClearTarget();

            Transform newTarget = enemyController.FindNearestVisibleTarget();
            if (newTarget != null && enemyController.SetTarget(newTarget))
                enemyController.StartTrackingTarget();

            return;
        }

        if (enemyController.CanCurrentlySeeTarget(CurrentTarget))
        {
            if (CurrentTargetState == EnemyTargetState.Searching ||
                CurrentTargetState == EnemyTargetState.Lost)
            {
                if (enemyController.ReacquireTarget())
                {
                    if (enemyController.IsTargetSearchBehaviourActive())
                        enemyController.StopTargetSearching();

                    targetSearchTimer = 0f;
                }
            }
            else
            {
                enemyController.UpdateLastKnownTargetPosition();
                if (CurrentTargetState == EnemyTargetState.Detected)
                    enemyController.StartTrackingTarget();
            }

            return;
        }

        if (CurrentTargetState == EnemyTargetState.Tracking && enemyController.LoseTarget())
        {
            if (enemyController.StartSearchingForTarget())
            {
                targetSearchTimer = enemyController.Definition.TargetSearchDuration;
                enemyController.StartTargetSearching(LastKnownTargetPosition);
            }
        }
    }

    private void UpdateTargetSearching(float deltaTime)
    {
        if (CurrentTargetState != EnemyTargetState.Searching)
            return;

        if (!enemyController.IsTargetSearchBehaviourActive())
        {
            if (enemyController.CompleteTargetSearching())
            {
                targetSearchTimer = 0f;
                targetScanTimer = 0f;
            }

            return;
        }

        targetSearchTimer -= deltaTime;
        if (targetSearchTimer > 0f)
            return;

        enemyController.StopTargetSearching();
        if (enemyController.CompleteTargetSearching())
        {
            targetSearchTimer = 0f;
            targetScanTimer = 0f;
        }
    }

    public virtual bool SetTarget(Transform target)
    {
        if (!IsTargetValidCandidate(target) || enemyController == null)
            return false;

        if (!enemyController.SetTarget(target))
            return false;

        enemyController.StartTrackingTarget();
        targetScanTimer = 0f;
        targetSearchTimer = 0f;
        return true;
    }

    public virtual void ClearTarget()
    {
        if (enemyController == null)
            return;

        if (enemyController.IsTargetSearchBehaviourActive())
            enemyController.StopTargetSearching();

        targetSearchTimer = 0f;
        enemyController.ClearTarget();
    }

    public bool IsTargetValid()
    {
        Transform target = CurrentTarget;
        return target != null && target.gameObject.activeInHierarchy &&
               target.root.gameObject.activeInHierarchy;
    }

    protected Vector2 GetTargetPosition()
    {
        return IsTargetValid() ? (Vector2)CurrentTarget.position : (Vector2)transform.position;
    }

    protected float GetDistanceToTarget()
    {
        return IsTargetValid()
            ? Vector2.Distance(transform.position, CurrentTarget.position)
            : float.PositiveInfinity;
    }

    private void ChaseTarget(Transform target)
    {
        Vector2 direction = GetTargetMovementDirection(target);
        if (direction.sqrMagnitude <= 0.0001f)
        {
            enemyController.StopNormalMovement();
            enemyController.SetAIState(EnemyState.Chase);
            return;
        }

        enemyController.SetAIState(EnemyState.Chase);
        enemyController.Move(direction);
    }

    private Vector2 GetTargetMovementDirection(Transform target)
    {
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        EnemyDefinition definition = enemyController.Definition;
        bool canMoveVertically = definition.HasCapability(EnemyCapability.Flying) ||
                                 definition.HasCapability(EnemyCapability.Swimming);

        if (!canMoveVertically)
            direction.y = 0f;

        return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.zero;
    }

    private Vector2 GetChargeTargetPosition(Transform target)
    {
        Vector2 targetPosition = target.position;
        EnemyDefinition definition = enemyController.Definition;
        bool canMoveVertically = definition.HasCapability(EnemyCapability.Flying) ||
                                 definition.HasCapability(EnemyCapability.Swimming);

        if (!canMoveVertically)
            targetPosition.y = transform.position.y;

        return targetPosition;
    }

    private bool IsTargetValidCandidate(Transform target)
    {
        return target != null && target != transform && !target.IsChildOf(transform) &&
               target.gameObject.activeInHierarchy && target.root.gameObject.activeInHierarchy;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
