using UnityEngine;

public sealed class EnemyPatrolSystem
{
    private EnemyController controller;
    private EnemyPatrolSettings settings;

    private Vector2 patrolOrigin;
    private int waypointIndex;
    private int waypointDirection = 1;
    private float waitTimer;
    private float baseMovementSpeed;

    public bool IsInitialized => controller != null && settings != null;
    public bool IsPatrolling { get; private set; }

    public void Initialize(EnemyController controller, EnemyPatrolSettings settings)
    {
        StopPatrol();

        if (controller == null || settings == null)
        {
            Debug.LogError("EnemyPatrolSystem requires an EnemyController and settings.");
            this.controller = null;
            this.settings = null;
            return;
        }

        this.controller = controller;
        this.settings = settings.CreateValidatedCopy();
        patrolOrigin = controller.transform.position;
        baseMovementSpeed = controller.Definition != null
            ? controller.Definition.MovementSpeed
            : 0f;
    }

    public bool CanPatrol =>
        IsInitialized &&
        settings.Enabled &&
        settings.LocalWaypoints != null &&
        settings.LocalWaypoints.Length >= 2;

    public bool StartPatrol()
    {
        if (!CanPatrol || controller.IsDead || controller.IsMovementLocked ||
            controller.IsHurting || controller.IsAttackInProgress ||
            controller.IsSpecialMovementActive ||
            (controller.TargetSystem != null && controller.TargetSystem.HasTarget))
            return false;

        if (IsPatrolling)
            return true;

        waypointIndex = 0;
        waypointDirection = 1;
        waitTimer = 0f;
        IsPatrolling = true;

        controller.SetMovementSpeed(baseMovementSpeed * settings.SpeedMultiplier);
        controller.SetAIState(EnemyState.Patrol);
        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsPatrolling || controller == null)
            return;

        if (controller.IsDead || !CanPatrol)
        {
            StopPatrol();
            return;
        }

        if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= deltaTime;
            if (waitTimer > 0f)
                return;

            AdvanceWaypoint();
            return;
        }

        Vector2 destination = patrolOrigin + settings.LocalWaypoints[waypointIndex];
        Vector2 offset = destination - (Vector2)controller.transform.position;
        float arrivalDistance = settings.ArrivalDistance;
        bool isGroundBased = !controller.Definition.HasCapability(EnemyCapability.Flying) &&
                             !controller.Definition.HasCapability(EnemyCapability.Swimming);
        float distanceToWaypoint = isGroundBased
            ? Mathf.Abs(offset.x)
            : offset.magnitude;

        if (distanceToWaypoint <= arrivalDistance)
        {
            controller.StopNormalMovement();

            if (settings.WaitAtWaypoint <= 0f)
                AdvanceWaypoint();
            else
                waitTimer = settings.WaitAtWaypoint;

            return;
        }

        Vector2 direction = isGroundBased
            ? new Vector2(Mathf.Sign(offset.x), 0f)
            : offset.normalized;

        if (!isGroundBased && offset.sqrMagnitude <= 0.0001f)
        {
            AdvanceWaypoint();
            return;
        }

        if (Mathf.Abs(direction.x) > 0.001f)
        {
            controller.Face(direction.x < 0f
                ? EnemyFacingDirection.Left
                : EnemyFacingDirection.Right);
        }

        controller.Move(direction);
    }

    private void AdvanceWaypoint()
    {
        int count = settings.LocalWaypoints.Length;
        int nextIndex = waypointIndex + waypointDirection;

        if (settings.PingPong)
        {
            if (nextIndex < 0 || nextIndex >= count)
            {
                waypointDirection *= -1;
                nextIndex = waypointIndex + waypointDirection;
            }
        }
        else if (nextIndex >= count)
        {
            nextIndex = 0;
        }

        waypointIndex = nextIndex;
    }

    public void StopPatrol()
    {
        if (!IsPatrolling)
            return;

        IsPatrolling = false;
        waitTimer = 0f;

        if (controller != null)
        {
            controller.StopNormalMovement();
            controller.SetMovementSpeed(baseMovementSpeed);
            controller.RefreshAIStateAfterMovement();
        }
    }
}
