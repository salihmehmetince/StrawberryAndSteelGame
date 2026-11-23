using UnityEngine;

public sealed class EnemyRetreatSystem
{
    private EnemyController controller;
    private EnemyRetreatSettings settings;
    private Transform target;
    private float baseMovementSpeed;
    private float remainingDuration;
    private float cooldownTimer;
    private bool stopAtDesiredDistance;

    public bool IsInitialized => controller != null && settings != null;
    public bool IsRetreating { get; private set; }
    public float CooldownRemaining => cooldownTimer;

    public void Initialize(EnemyController controller, EnemyRetreatSettings settings)
    {
        StopRetreat();

        if (controller == null || settings == null)
        {
            Debug.LogError("EnemyRetreatSystem requires an EnemyController and settings.");
            this.controller = null;
            this.settings = null;
            return;
        }

        this.controller = controller;
        this.settings = settings.CreateValidatedCopy();
        cooldownTimer = 0f;
        stopAtDesiredDistance = false;
        baseMovementSpeed = controller.Definition != null
            ? controller.Definition.MovementSpeed
            : 0f;
    }

    public bool ShouldRetreat(Transform target, float healthPercentage)
    {
        if (!CanStartRetreat(target) || cooldownTimer > 0f)
            return false;

        float distance = Vector2.Distance(controller.transform.position, target.position);
        bool isTooClose = settings.TriggerDistance > 0f &&
                          distance <= settings.TriggerDistance;
        bool healthIsLow = settings.HealthThresholdPercent > 0f &&
                           healthPercentage * 100f <= settings.HealthThresholdPercent;

        return isTooClose || healthIsLow;
    }

    public bool TryStartRetreat(Transform target, float healthPercentage)
    {
        return ShouldRetreat(target, healthPercentage) && StartRetreat(target);
    }

    public bool StartRetreat(Transform target)
    {
        if (IsRetreating)
        {
            if (CanStartRetreat(target))
                this.target = target;

            return this.target != null;
        }

        if (!CanStartRetreat(target) || cooldownTimer > 0f)
            return false;

        this.target = target;
        remainingDuration = settings.MaximumDuration;
        stopAtDesiredDistance = Vector2.Distance(
            controller.transform.position,
            target.position) < settings.DesiredDistance;
        IsRetreating = true;

        controller.SetMovementSpeed(baseMovementSpeed * settings.SpeedMultiplier);
        controller.SetAIState(EnemyState.Retreat);
        return true;
    }

    private bool CanStartRetreat(Transform candidate)
    {
        return IsInitialized &&
               settings.Enabled &&
               controller != null &&
               !controller.IsDead &&
               !controller.IsMovementLocked &&
               !controller.IsHurting &&
               !controller.IsAttackInProgress &&
               !controller.IsSpecialMovementActive &&
               !controller.IsTargetSearchBehaviourActive() &&
               candidate != null &&
               candidate.gameObject.activeInHierarchy;
    }

    public void Update(float deltaTime)
    {
        if (!IsRetreating)
        {
            if (cooldownTimer > 0f &&
                !float.IsNaN(deltaTime) &&
                !float.IsInfinity(deltaTime) &&
                deltaTime > 0f)
            {
                cooldownTimer = Mathf.Max(0f, cooldownTimer - deltaTime);
            }

            return;
        }

        if (controller == null || controller.IsDead || target == null ||
            !target.gameObject.activeInHierarchy)
        {
            StopRetreat();
            return;
        }

        if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
            return;

        Vector2 position = controller.transform.position;
        Vector2 targetPosition = target.position;
        float currentDistance = Vector2.Distance(position, targetPosition);

        if (stopAtDesiredDistance && currentDistance >= settings.DesiredDistance)
        {
            FinishRetreat();
            return;
        }

        remainingDuration -= deltaTime;
        if (remainingDuration <= 0f)
        {
            FinishRetreat();
            return;
        }

        Vector2 direction = position - targetPosition;
        bool isGroundBased = !controller.Definition.HasCapability(EnemyCapability.Flying) &&
                             !controller.Definition.HasCapability(EnemyCapability.Swimming);

        if (isGroundBased)
            direction.y = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            direction = controller.IsFacing(EnemyFacingDirection.Left)
                ? Vector2.right
                : Vector2.left;
        }

        direction.Normalize();

        if (Mathf.Abs(direction.x) > 0.001f)
        {
            controller.Face(direction.x < 0f
                ? EnemyFacingDirection.Left
                : EnemyFacingDirection.Right);
        }

        controller.Move(direction);
    }

    private void FinishRetreat()
    {
        StopRetreat();
        cooldownTimer = settings.Cooldown;
    }

    public void StopRetreat()
    {
        if (!IsRetreating)
            return;

        IsRetreating = false;
        target = null;
        remainingDuration = 0f;
        stopAtDesiredDistance = false;

        if (controller != null)
        {
            controller.StopNormalMovement();
            controller.SetMovementSpeed(baseMovementSpeed);
            controller.RefreshAIStateAfterMovement();
        }
    }
}
