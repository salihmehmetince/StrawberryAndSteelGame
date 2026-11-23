
public class EnemySpecialAttackSequenceSystem
{
    private EnemyAttackStateSystem stateSystem;

    public bool IsInitialized => stateSystem != null;

    public EnemyAttackState CurrentState =>
        stateSystem != null
            ? stateSystem.CurrentState
            : EnemyAttackState.Idle;

    public bool IsSequenceRunning =>
        IsInitialized &&
        CurrentState != EnemyAttackState.Idle &&
        CurrentState != EnemyAttackState.Interrupted;

    public void Initialize(EnemyAttackStateSystem stateSystem)
    {
        this.stateSystem = null;

        if (stateSystem == null)
        {
            UnityEngine.Debug.LogError(
                "EnemySpecialAttackSequenceSystem requires an EnemyAttackStateSystem.");

            return;
        }

        this.stateSystem = stateSystem;
        this.stateSystem.Reset();
    }

    public bool StartSequence()
    {
        if (!IsInitialized || !stateSystem.CanStartAttack())
            return false;

        return stateSystem.StartPreparing();
    }

    public void StartAttackPhaseFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Preparing)
        {
            return;
        }

        stateSystem.StartAttacking();
    }

    public void StartRecoveryPhaseFromAnimation()
    {
        if (!IsInitialized ||
            CurrentState != EnemyAttackState.Attacking)
        {
            return;
        }

        stateSystem.StartRecovery();
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
    }

    public void Reset()
    {
        if (!IsInitialized)
            return;

        stateSystem.Reset();
    }

    public void Clear()
    {
        stateSystem = null;
    }
}
