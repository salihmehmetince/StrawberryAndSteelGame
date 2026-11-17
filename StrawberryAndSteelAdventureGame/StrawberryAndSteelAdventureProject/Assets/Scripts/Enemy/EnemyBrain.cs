
using UnityEngine;

public abstract class EnemyBrain : MonoBehaviour
{
    protected EnemyController enemyController;

    [Header("Target Scanning")]
    [SerializeField] private float targetScanInterval;

    private float targetScanTimer;
    private float targetSearchTimer;

    public bool IsInitialized { get; private set; }

    public Transform CurrentTarget =>
        enemyController != null
            ? enemyController.TargetSystem?.CurrentTarget
            : null;

    public bool HasTarget => CurrentTarget != null;

    public Vector2 LastKnownTargetPosition =>
        enemyController != null &&
        enemyController.TargetSystem != null
            ? enemyController.TargetSystem.LastKnownPosition
            : Vector2.zero;

    public bool HasLastKnownTargetPosition =>
        enemyController != null &&
        enemyController.TargetSystem != null &&
        enemyController.TargetSystem.HasLastKnownPosition;

    public EnemyTargetState CurrentTargetState =>
        enemyController != null &&
        enemyController.TargetSystem != null
            ? enemyController.TargetSystem.CurrentState
            : EnemyTargetState.NoTarget;

    protected virtual void Start()
    {
        enemyController = GetComponent<EnemyController>();

        if (enemyController == null)
        {
            Debug.LogError(
                $"{GetType().Name} requires an EnemyController.");

            enabled = false;
            return;
        }

        if (!enemyController.IsInitialized)
        {
            Debug.LogError(
                $"{GetType().Name}: EnemyController is not initialized.");

            enabled = false;
            return;
        }

        if (targetScanInterval <= 0f)
        {
            Debug.LogError(
                $"{GetType().Name}: Target Scan Interval must be positive.");

            enabled = false;
            return;
        }

        targetScanTimer = 0f;

        InitializeBrain();

        IsInitialized = true;
    }

    protected virtual void Update()
    {
        if (!IsInitialized)
            return;

        UpdateTargetScanning();

        UpdateTargetSearching(Time.deltaTime);

        if (enemyController.IsTargetSearchBehaviourActive())
            return;

        Think(Time.deltaTime);
    }

    protected abstract void InitializeBrain();

    protected abstract void Think(float deltaTime);

    private void UpdateTargetScanning()
    {
        targetScanTimer -= Time.deltaTime;

        if (targetScanTimer > 0f)
            return;

        targetScanTimer = targetScanInterval;

        if (!IsTargetValid())
        {
            if (enemyController.IsTargetSearchBehaviourActive())
                enemyController.StopTargetSearching();

            targetSearchTimer = 0f;

            enemyController.ClearTarget();

            Transform newTarget =
                enemyController.FindNearestVisibleTarget();

            if (newTarget != null && SetTarget(newTarget))
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
            }

            return;
        }

        if (CurrentTargetState == EnemyTargetState.Tracking)
        {
            if (enemyController.LoseTarget())
            {
                if (enemyController.StartSearchingForTarget())
                {
                    targetSearchTimer =
                        enemyController.Definition.TargetSearchDuration;

                    enemyController.StartTargetSearching(
                        LastKnownTargetPosition);
                }
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
        if (target == null)
            return false;

        if (target == transform || target.IsChildOf(transform))
            return false;

        if (enemyController == null)
            return false;

        return enemyController.SetTarget(target);
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
        return CurrentTarget != null &&
               CurrentTarget.gameObject.activeInHierarchy;
    }

    protected Vector2 GetTargetPosition()
    {
        if (!IsTargetValid())
            return transform.position;

        return CurrentTarget.position;
    }

    protected float GetDistanceToTarget()
    {
        if (!IsTargetValid())
            return float.PositiveInfinity;

        return Vector2.Distance(
            transform.position,
            CurrentTarget.position);
    }
}
