
public class EnemySpecialAttackSequenceSystem
{
    private EnemyAttackStateSystem stateSystem;
    private EnemySpecialAttackDefinition activeDefinition;
    private float fallbackGraceDuration;
    private float fallbackTimer;

    public bool IsInitialized => stateSystem != null && fallbackGraceDuration >= 0f;

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
        float fallbackGraceDuration = 0.5f)
    {
        this.stateSystem = null;
        activeDefinition = null;
        this.fallbackGraceDuration = 0f;
        fallbackTimer = 0f;

        if (stateSystem == null ||
            float.IsNaN(fallbackGraceDuration) ||
            float.IsInfinity(fallbackGraceDuration) ||
            fallbackGraceDuration < 0f)
        {
            UnityEngine.Debug.LogError(
                "EnemySpecialAttackSequenceSystem requires an EnemyAttackStateSystem and a finite, non-negative fallback grace duration.");

            return;
        }

        this.stateSystem = stateSystem;
        this.fallbackGraceDuration = fallbackGraceDuration;
        this.stateSystem.Reset();
    }

    public bool StartSequence(EnemySpecialAttackDefinition definition)
    {
        if (!IsInitialized || definition == null || !stateSystem.CanStartAttack() ||
            !IsValidDuration(definition.PreparationDuration) ||
            !IsValidDuration(definition.AttackDuration) ||
            !IsValidDuration(definition.RecoveryDuration))
            return false;

        if (!stateSystem.StartPreparing())
            return false;

        activeDefinition = definition;
        SetFallback(definition.PreparationDuration);

        if (definition.PreparationDuration <= 0f)
            StartAttackPhaseFromAnimation();

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized || !IsSequenceRunning ||
            float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
        {
            return;
        }

        fallbackTimer -= deltaTime;
        if (fallbackTimer > 0f)
            return;

        switch (CurrentState)
        {
            case EnemyAttackState.Preparing:
                StartAttackPhaseFromAnimation();
                break;
            case EnemyAttackState.Attacking:
                StartRecoveryPhaseFromAnimation();
                break;
            case EnemyAttackState.Recovery:
                CompleteRecoveryFromAnimation();
                break;
        }
    }

    public void StartAttackPhaseFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Preparing)
        {
            return;
        }

        if (!stateSystem.StartAttacking())
            return;

        SetFallback(activeDefinition.AttackDuration);

        if (activeDefinition.AttackDuration <= 0f)
            StartRecoveryPhaseFromAnimation();
    }

    public void StartRecoveryPhaseFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Attacking)
        {
            return;
        }

        if (!stateSystem.StartRecovery())
            return;

        SetFallback(activeDefinition.RecoveryDuration);

        if (activeDefinition.RecoveryDuration <= 0f)
            CompleteRecoveryFromAnimation();
    }

    public void CompleteRecoveryFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Recovery)
        {
            return;
        }

        if (!stateSystem.StartCooldown())
            return;

        stateSystem.CompleteCooldown();
        activeDefinition = null;
        fallbackTimer = 0f;
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
        activeDefinition = null;
        fallbackTimer = 0f;
    }

    public void Reset()
    {
        if (!IsInitialized)
            return;

        stateSystem.Reset();
        activeDefinition = null;
        fallbackTimer = 0f;
    }

    public void Clear()
    {
        stateSystem = null;
        activeDefinition = null;
        fallbackGraceDuration = 0f;
        fallbackTimer = 0f;
    }

    private void SetFallback(float duration)
    {
        fallbackTimer = duration + fallbackGraceDuration;
    }

    private static bool IsValidDuration(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f;
    }
}
