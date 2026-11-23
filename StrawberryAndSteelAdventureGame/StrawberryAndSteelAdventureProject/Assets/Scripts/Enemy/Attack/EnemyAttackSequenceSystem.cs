using UnityEngine;

public class EnemyAttackSequenceSystem
{
    private EnemyAttackStateSystem stateSystem;
    private EnemyAttackTimingSystem timingSystem;
    private EnemyAttackCooldownSystem cooldownSystem;
    private bool useAnimationEvents;
    private float animationEventFallbackGraceDuration;
    private float animationEventFallbackTimer;

    public bool IsInitialized =>
        stateSystem != null &&
        timingSystem != null &&
        cooldownSystem != null &&
        timingSystem.IsInitialized &&
        cooldownSystem.IsInitialized;

    public EnemyAttackState CurrentState => stateSystem != null
        ? stateSystem.CurrentState
        : EnemyAttackState.Idle;

    public bool IsSequenceRunning =>
        IsInitialized &&
        CurrentState != EnemyAttackState.Idle &&
        CurrentState != EnemyAttackState.Interrupted &&
        CurrentState != EnemyAttackState.Cooldown;

    public void Initialize(
        EnemyAttackStateSystem stateSystem,
        EnemyAttackTimingSystem timingSystem,
        EnemyAttackCooldownSystem cooldownSystem,
        bool useAnimationEvents,
        float animationEventFallbackGraceDuration = 0.5f)
    {
        Clear();

        if (stateSystem == null || timingSystem == null || cooldownSystem == null ||
            !timingSystem.IsInitialized || !cooldownSystem.IsInitialized ||
            !IsFinite(animationEventFallbackGraceDuration) ||
            animationEventFallbackGraceDuration < 0f)
        {
            Debug.LogError("EnemyAttackSequenceSystem received invalid initialization values.");
            return;
        }

        this.stateSystem = stateSystem;
        this.timingSystem = timingSystem;
        this.cooldownSystem = cooldownSystem;
        this.useAnimationEvents = useAnimationEvents;
        this.animationEventFallbackGraceDuration = animationEventFallbackGraceDuration;
    }

    public bool StartSequence()
    {
        if (!IsInitialized || !stateSystem.CanStartAttack() || cooldownSystem.IsOnCooldown)
            return false;

        if (!stateSystem.StartPreparing())
            return false;

        timingSystem.StartPreparation();
        if (useAnimationEvents)
            SetAnimationEventFallback(timingSystem.PreparationDuration);

        if (timingSystem.PreparationDuration <= 0f)
            StartAttackPhase();

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized || !IsFinite(deltaTime) || deltaTime < 0f)
            return;

        if (CurrentState == EnemyAttackState.Cooldown)
        {
            cooldownSystem.Update(deltaTime);
            if (!cooldownSystem.IsOnCooldown)
                stateSystem.CompleteCooldown();
            return;
        }

        if (!IsSequenceRunning)
            return;

        if (useAnimationEvents)
        {
            animationEventFallbackTimer -= deltaTime;
            if (animationEventFallbackTimer <= 0f)
                AdvanceFromAnimationFallback();
            return;
        }

        if (!timingSystem.Update(deltaTime))
            return;

        switch (CurrentState)
        {
            case EnemyAttackState.Preparing:
                StartAttackPhase();
                break;
            case EnemyAttackState.Attacking:
                StartRecoveryPhase();
                break;
            case EnemyAttackState.Recovery:
                StartCooldownPhase();
                break;
        }
    }

    public void StartAttackPhaseFromAnimation()
    {
        if (IsInitialized && useAnimationEvents && CurrentState == EnemyAttackState.Preparing)
            StartAttackPhase();
    }

    public void StartRecoveryPhaseFromAnimation()
    {
        if (IsInitialized && useAnimationEvents && CurrentState == EnemyAttackState.Attacking)
            StartRecoveryPhase();
    }

    public void CompleteRecoveryFromAnimation()
    {
        if (IsInitialized && useAnimationEvents && CurrentState == EnemyAttackState.Recovery)
            StartCooldownPhase();
    }

    private void AdvanceFromAnimationFallback()
    {
        switch (CurrentState)
        {
            case EnemyAttackState.Preparing:
                StartAttackPhase();
                break;
            case EnemyAttackState.Attacking:
                StartRecoveryPhase();
                break;
            case EnemyAttackState.Recovery:
                StartCooldownPhase();
                break;
        }
    }

    private void StartAttackPhase()
    {
        if (!IsInitialized || CurrentState != EnemyAttackState.Preparing ||
            !stateSystem.StartAttacking())
            return;

        timingSystem.StartAttack();
        if (useAnimationEvents)
            SetAnimationEventFallback(timingSystem.AttackDuration);

        if (timingSystem.AttackDuration <= 0f)
            StartRecoveryPhase();
    }

    private void StartRecoveryPhase()
    {
        if (!IsInitialized || CurrentState != EnemyAttackState.Attacking ||
            !stateSystem.StartRecovery())
            return;

        timingSystem.StartRecovery();
        if (useAnimationEvents)
            SetAnimationEventFallback(timingSystem.RecoveryDuration);

        if (timingSystem.RecoveryDuration <= 0f)
            StartCooldownPhase();
    }

    private void StartCooldownPhase()
    {
        if (!IsInitialized || CurrentState != EnemyAttackState.Recovery ||
            !stateSystem.StartCooldown())
            return;

        timingSystem.Reset();
        animationEventFallbackTimer = 0f;

        if (!cooldownSystem.TryStartCooldown() || !cooldownSystem.IsOnCooldown)
            stateSystem.CompleteCooldown();
    }

    public void Interrupt()
    {
        if (!IsInitialized || CurrentState == EnemyAttackState.Idle ||
            CurrentState == EnemyAttackState.Interrupted)
            return;

        stateSystem.Interrupt();
        timingSystem.Reset();
        cooldownSystem.ResetCooldown();
        animationEventFallbackTimer = 0f;
    }

    public void Reset()
    {
        if (!IsInitialized)
            return;

        stateSystem.Reset();
        timingSystem.Reset();
        cooldownSystem.ResetCooldown();
        animationEventFallbackTimer = 0f;
    }

    public void Clear()
    {
        stateSystem = null;
        timingSystem = null;
        cooldownSystem = null;
        useAnimationEvents = false;
        animationEventFallbackGraceDuration = 0f;
        animationEventFallbackTimer = 0f;
    }

    private void SetAnimationEventFallback(float phaseDuration)
    {
        animationEventFallbackTimer = phaseDuration + animationEventFallbackGraceDuration;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}