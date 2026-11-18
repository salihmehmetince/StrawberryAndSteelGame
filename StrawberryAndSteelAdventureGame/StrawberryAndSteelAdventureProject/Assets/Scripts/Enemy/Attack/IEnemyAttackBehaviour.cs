
public interface IEnemyAttackBehaviour
{
    bool IsInitialized { get; }

    bool TryAttack();
}
