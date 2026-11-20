
using UnityEngine;

public class EnemyAttackSequenceSystem
{
    private EnemyAttackStateSystem stateSystem;
    private EnemyAttackTimingSystem timingSystem;
    private EnemyAttackCooldownSystem cooldownSystem;
    private bool useAnimationEvents;

    public bool IsInitialized =>
        stateSystem != null &&
        timingSystem != null &&
        cooldownSystem != null &&
        timingSystem.IsInitialized &&
        cooldownSystem.IsInitialized;

    public EnemyAttackState CurrentState =>
        stateSystem != null
            ? stateSystem.CurrentState
            : EnemyAttackState.Idle;

    public bool IsSequenceRunning =>
        IsInitialized &&
        CurrentState != EnemyAttackState.Idle &&
        CurrentState != EnemyAttackState.Interrupted;

    public void Initialize(
        EnemyAttackStateSystem stateSystem,
        EnemyAttackTimingSystem timingSystem,
        EnemyAttackCooldownSystem cooldownSystem,
        bool useAnimationEvents)
    {
        // Önceki yapýlandýrmayý temizle.
        this.stateSystem = null;
        this.timingSystem = null;
        this.cooldownSystem = null;
        this.useAnimationEvents = false;

        if (stateSystem == null ||
            timingSystem == null ||
            cooldownSystem == null)
        {
            Debug.LogError(
                "EnemyAttackSequenceSystem requires all attack systems.");

            return;
        }

        if (!timingSystem.IsInitialized ||
            !cooldownSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackSequenceSystem received an uninitialized system.");

            return;
        }

        this.stateSystem = stateSystem;
        this.timingSystem = timingSystem;
        this.cooldownSystem = cooldownSystem;
        this.useAnimationEvents = useAnimationEvents;
    }

    public bool StartSequence()
    {
        if (!IsInitialized ||
            !stateSystem.CanStartAttack() ||
            cooldownSystem.IsOnCooldown)
        {
            return false;
        }

        if (!stateSystem.StartPreparing())
            return false;

        timingSystem.StartPreparation();

        if (!useAnimationEvents &&
            timingSystem.PreparationDuration <= 0f)
        {
            StartAttackPhase();
        }

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized ||
            float.IsNaN(deltaTime) ||
            float.IsInfinity(deltaTime) ||
            deltaTime < 0f)
        {
            return;
        }

        if (CurrentState == EnemyAttackState.Cooldown)
        {
            cooldownSystem.Update(deltaTime);

            if (!cooldownSystem.IsOnCooldown)
                stateSystem.CompleteCooldown();

            return;
        }

        if (useAnimationEvents || !IsSequenceRunning)
            return;

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
        if (!IsInitialized ||
            !useAnimationEvents ||
            CurrentState != EnemyAttackState.Preparing)
        {
            return;
        }

        StartAttackPhase();
    }

    public void StartRecoveryPhaseFromAnimation()
    {
        if (!IsInitialized ||
            !useAnimationEvents ||
            CurrentState != EnemyAttackState.Attacking)
        {
            return;
        }

        StartRecoveryPhase();
    }

    public void CompleteRecoveryFromAnimation()
    {
        if (!IsInitialized ||
            !useAnimationEvents ||
            CurrentState != EnemyAttackState.Recovery)
        {
            return;
        }

        StartCooldownPhase();
    }

    private void StartAttackPhase()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Preparing)
        {
            return;
        }

        if (!stateSystem.StartAttacking())
            return;

        timingSystem.StartAttack();
    }

    private void StartRecoveryPhase()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Attacking)
        {
            return;
        }

        if (!stateSystem.StartRecovery())
            return;

        timingSystem.StartRecovery();

        if (timingSystem.RecoveryDuration <= 0f)
            StartCooldownPhase();
    }

    private void StartCooldownPhase()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Recovery)
        {
            return;
        }

        if (!stateSystem.StartCooldown())
            return;

        timingSystem.Reset();

        if (!cooldownSystem.TryStartCooldown() ||
            !cooldownSystem.IsOnCooldown)
        {
            stateSystem.CompleteCooldown();
        }
    }

    public void Interrupt()
    {
        if (!IsInitialized ||
            CurrentState == EnemyAttackState.Idle ||
            CurrentState == EnemyAttackState.Interrupted)
        {
            return;
        }

        stateSystem.Interrupt();
        timingSystem.Reset();
        cooldownSystem.ResetCooldown();
    }

    public void Reset()
    {
        if (!IsInitialized)
            return;

        stateSystem.Reset();
        timingSystem.Reset();
        cooldownSystem.ResetCooldown();
    }
}
