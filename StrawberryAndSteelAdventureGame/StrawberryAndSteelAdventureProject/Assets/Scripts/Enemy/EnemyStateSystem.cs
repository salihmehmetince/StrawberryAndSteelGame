
public class EnemyStateSystem
{
    public EnemyState CurrentState { get; private set; }

    public bool IsDead => CurrentState == EnemyState.Dead;

    public EnemyStateSystem()
    {
        CurrentState = EnemyState.Idle;
    }

    public bool SetState(EnemyState newState)
    {
        if (IsDead)
            return false;

        if (CurrentState == newState)
            return false;

        CurrentState = newState;

        return true;
    }

    public void ResetState()
    {
        CurrentState = EnemyState.Idle;
    }

}
