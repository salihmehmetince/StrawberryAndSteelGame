
public class EnemyAttackStateSystem
{
    public EnemyAttackState CurrentState { get; private set; } =
        EnemyAttackState.Idle;

    public bool IsIdle =>
        CurrentState == EnemyAttackState.Idle;

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

    public bool CanStartAttack()
    {
        return IsIdle;
    }

    public bool StartPreparing()
    {
        if (!CanStartAttack())
            return false;

        SetState(EnemyAttackState.Preparing);
        return true;
    }

    public bool StartAttacking()
    {
        if (!IsPreparing)
            return false;

        SetState(EnemyAttackState.Attacking);
        return true;
    }

    public bool StartRecovery()
    {
        if (!IsAttacking)
            return false;

        SetState(EnemyAttackState.Recovery);
        return true;
    }

    public bool StartCooldown()
    {
        if (!IsRecovering)
            return false;

        SetState(EnemyAttackState.Cooldown);
        return true;
    }

    public bool CompleteCooldown()
    {
        if (!IsOnCooldown)
            return false;

        SetState(EnemyAttackState.Idle);
        return true;
    }

    public void Interrupt()
    {
        if (IsIdle || IsInterrupted)
            return;

        SetState(EnemyAttackState.Interrupted);
    }

    public void Reset()
    {
        SetState(EnemyAttackState.Idle);
    }

    private void SetState(EnemyAttackState newState)
    {
        CurrentState = newState;
    }
}
