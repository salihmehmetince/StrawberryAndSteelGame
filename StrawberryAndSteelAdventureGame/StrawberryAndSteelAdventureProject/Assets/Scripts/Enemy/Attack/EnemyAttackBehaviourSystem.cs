
using UnityEngine;

public class EnemyAttackBehaviourSystem
{
    private IEnemyAttackBehaviour activeBehaviour;

    public bool IsInitialized =>
        activeBehaviour != null &&
        activeBehaviour.IsInitialized;

    public IEnemyAttackBehaviour ActiveBehaviour =>
        activeBehaviour;

    public bool SetBehaviour(IEnemyAttackBehaviour behaviour)
    {
        if (behaviour == null)
        {
            Debug.LogError(
                "EnemyAttackBehaviourSystem cannot accept a null behaviour.");

            return false;
        }

        if (!behaviour.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackBehaviourSystem received an uninitialized behaviour.");

            return false;
        }

        activeBehaviour = behaviour;

        return true;
    }

    public bool TryAttack()
    {
        if (!IsInitialized)
            return false;

        return activeBehaviour.TryAttack();
    }

    public void ClearBehaviour()
    {
        activeBehaviour = null;
    }
}
