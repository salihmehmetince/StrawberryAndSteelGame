
public class EnemyAttackStateSystem
{
    public EnemyAttackState CurrentState { get; private set; } =
        EnemyAttackState.Idle;

    public bool IsAttacking =>
        CurrentState == EnemyAttackState.Attacking;

    public bool IsPreparing =>
        CurrentState == EnemyAttackState.Preparing;

    public bool IsRecovering =>
        CurrentState == EnemyAttackState.Recovery;

    public bool IsOnCooldown =>
        CurrentState == EnemyAttackState.Cooldown;

    public bool IsInterrupted =>
        CurrentState == EnemyAttackState.Interrupted;

    public void SetState(EnemyAttackState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
    }

    public bool CanStartAttack()
    {
        return CurrentState == EnemyAttackState.Idle;
    }

    public void StartPreparing()
    {
        if (!CanStartAttack())
            return;

        SetState(EnemyAttackState.Preparing);
    }

    public void StartAttacking()
    {
        if (CurrentState != EnemyAttackState.Preparing &&
            CurrentState != EnemyAttackState.Idle)
        {
            return;
        }

        SetState(EnemyAttackState.Attacking);
    }

    public void StartRecovery()
    {
        if (CurrentState != EnemyAttackState.Attacking)
            return;

        SetState(EnemyAttackState.Recovery);
    }

    public void StartCooldown()
    {
        if (CurrentState != EnemyAttackState.Recovery &&
            CurrentState != EnemyAttackState.Attacking)
        {
            return;
        }

        SetState(EnemyAttackState.Cooldown);
    }

    public void CompleteCooldown()
    {
        if (CurrentState != EnemyAttackState.Cooldown)
            return;

        SetState(EnemyAttackState.Idle);
    }

    public void Interrupt()
    {
        if (CurrentState == EnemyAttackState.Idle)
            return;

        SetState(EnemyAttackState.Interrupted);
    }

    public void Reset()
    {
        SetState(EnemyAttackState.Idle);
    }
}
