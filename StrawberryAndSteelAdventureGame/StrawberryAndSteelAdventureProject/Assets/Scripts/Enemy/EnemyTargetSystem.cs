
using UnityEngine;

public class EnemyTargetSystem
{
    private Transform currentTarget;

    private Vector2 lastKnownPosition;
    private bool hasLastKnownPosition;

    public EnemyTargetState CurrentState { get; private set; }

    public Transform CurrentTarget => currentTarget;

    public Vector2 LastKnownPosition => lastKnownPosition;

    public bool HasTarget => currentTarget != null;

    public bool HasLastKnownPosition => hasLastKnownPosition;

    public EnemyTargetSystem()
    {
        CurrentState = EnemyTargetState.NoTarget;
    }

    public bool SetTarget(Transform target)
    {
        if (target == null || !target.gameObject.activeInHierarchy)
            return false;

        if (currentTarget == target)
            return false;

        currentTarget = target;

        UpdateLastKnownPosition();

        CurrentState = EnemyTargetState.Detected;

        return true;
    }

    public bool StartTracking()
    {
        if (currentTarget == null)
            return false;

        if (!currentTarget.gameObject.activeInHierarchy)
            return false;

        if (CurrentState != EnemyTargetState.Detected)
            return false;

        CurrentState = EnemyTargetState.Tracking;

        return true;
    }

    public void UpdateLastKnownPosition()
    {
        if (currentTarget == null)
            return;

        if (!currentTarget.gameObject.activeInHierarchy)
            return;

        lastKnownPosition = currentTarget.position;
        hasLastKnownPosition = true;
    }

    public bool LoseTarget()
    {
        if (currentTarget == null)
            return false;

        if (CurrentState != EnemyTargetState.Tracking)
            return false;

        UpdateLastKnownPosition();

        CurrentState = EnemyTargetState.Lost;

        return true;
    }

    public bool StartSearching()
    {
        if (CurrentState != EnemyTargetState.Lost)
            return false;

        if (!hasLastKnownPosition)
            return false;

        CurrentState = EnemyTargetState.Searching;

        return true;
    }

    public bool ReacquireTarget()
    {
        if (currentTarget == null)
            return false;

        if (!currentTarget.gameObject.activeInHierarchy)
            return false;

        if (CurrentState != EnemyTargetState.Lost &&
            CurrentState != EnemyTargetState.Searching)
            return false;

        UpdateLastKnownPosition();

        CurrentState = EnemyTargetState.Tracking;

        return true;
    }

    public bool CompleteSearching()
    {
        if (CurrentState != EnemyTargetState.Searching)
            return false;

        ClearTarget();

        return true;
    }

    public void ClearTarget()
    {
        currentTarget = null;

        lastKnownPosition = Vector2.zero;
        hasLastKnownPosition = false;

        CurrentState = EnemyTargetState.NoTarget;
    }

    public bool IsInState(EnemyTargetState state)
    {
        return CurrentState == state;
    }
}
