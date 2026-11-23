
using System.Collections.Generic;

public class EnemySpecialAttackBehaviourFactory
{
    private readonly Dictionary<
        EnemySpecialAttackType,
        IEnemySpecialAttackBehaviour> behaviours;

    public EnemySpecialAttackBehaviourFactory(
        IEnemySpecialAttackBehaviour meleeBehaviour,
        IEnemySpecialAttackBehaviour rangedBehaviour,
        IEnemySpecialAttackBehaviour areaBehaviour)
    {
        behaviours = new Dictionary<
            EnemySpecialAttackType,
            IEnemySpecialAttackBehaviour>();

        if (meleeBehaviour != null)
        {
            behaviours.Add(
                EnemySpecialAttackType.Melee,
                meleeBehaviour);
        }

        if (rangedBehaviour != null)
        {
            behaviours.Add(
                EnemySpecialAttackType.Ranged,
                rangedBehaviour);
        }

        if (areaBehaviour != null)
        {
            behaviours.Add(
                EnemySpecialAttackType.Area,
                areaBehaviour);
        }
    }

    public IEnemySpecialAttackBehaviour GetBehaviour(
        EnemySpecialAttackDefinition definition)
    {
        if (definition == null)
            return null;

        behaviours.TryGetValue(
            definition.AttackType,
            out IEnemySpecialAttackBehaviour behaviour);

        return behaviour;
    }

    public bool CanExecute(
        EnemySpecialAttackDefinition definition)
    {
        IEnemySpecialAttackBehaviour behaviour =
            GetBehaviour(definition);

        return behaviour != null &&
               behaviour.CanExecute(definition);
    }

    public void Execute(
        EnemySpecialAttackDefinition definition)
    {
        IEnemySpecialAttackBehaviour behaviour =
            GetBehaviour(definition);

        behaviour?.Execute(definition);
    }
}
