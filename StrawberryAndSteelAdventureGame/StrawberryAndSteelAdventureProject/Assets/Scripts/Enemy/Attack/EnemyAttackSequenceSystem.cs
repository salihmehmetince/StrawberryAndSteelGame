
using UnityEngine;

public class EnemyAttackSequenceSystem
{
    private EnemyAttackStateSystem stateSystem;
    private EnemyAttackTimingSystem timingSystem;

    public bool IsInitialized =>
        stateSystem != null &&
        timingSystem != null &&
        timingSystem.IsInitialized;

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
        EnemyAttackTimingSystem timingSystem)
    {
        if (stateSystem == null || timingSystem == null)
        {
            Debug.LogError(
                "EnemyAttackSequenceSystem requires all state and timing systems.");

            return;
        }

        if (!timingSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackSequenceSystem received an uninitialized timing system.");

            return;
        }

        this.stateSystem = stateSystem;
        this.timingSystem = timingSystem;
    }

    public bool StartSequence()
    {
        if (!IsInitialized || !stateSystem.CanStartAttack())
            return false;

        stateSystem.StartPreparing();
        timingSystem.StartPreparation();

        if (timingSystem.PreparationDuration <= 0f)
            StartAttackPhase();

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsSequenceRunning)
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
            CurrentState != EnemyAttackState.Preparing)
            return;

        StartAttackPhase();
    }

    public void StartRecoveryPhaseFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Attacking)
            return;

        StartRecoveryPhase();
    }

    public void CompleteRecoveryFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Recovery)
            return;

        StartCooldownPhase();
    }

    private void StartAttackPhase()
    {
        stateSystem.StartAttacking();
        timingSystem.StartAttack();
    }

    private void StartRecoveryPhase()
    {
        stateSystem.StartRecovery();
        timingSystem.StartRecovery();
    }

    private void StartCooldownPhase()
    {
        stateSystem.StartCooldown();
        timingSystem.Reset();
    }

    public void CompleteCooldown()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Cooldown)
            return;

        stateSystem.CompleteCooldown();
        timingSystem.Reset();
    }

    public void Interrupt()
    {
        if (!IsInitialized)
            return;

        stateSystem.Interrupt();
        timingSystem.Reset();
    }

    public void Reset()
    {
        if (!IsInitialized)
            return;

        stateSystem.Reset();
        timingSystem.Reset();
    }
}
