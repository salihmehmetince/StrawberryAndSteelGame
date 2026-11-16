
public interface IEnemyMovementAction
{
    EnemyMovementActionPriority Priority { get; }

    bool IsActive { get; }

    void Update(float deltaTime);

    void Stop();
}
