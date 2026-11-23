
public interface IEnemySpecialAttackBehaviour
{
    bool CanExecute(EnemySpecialAttackDefinition definition);

    void Execute(EnemySpecialAttackDefinition definition);
}
