
using UnityEngine;

public class EnemySpecialAttackAnimationEventHandler : MonoBehaviour
{
    private EnemySpecialAttackSystem specialAttackSystem;
    private EnemySpecialAttackBehaviourFactory behaviourFactory;

    public void Initialize(
        EnemySpecialAttackSystem specialAttackSystem,
        EnemySpecialAttackBehaviourFactory behaviourFactory)
    {
        if (specialAttackSystem == null)
        {
            Debug.LogError(
                "EnemySpecialAttackAnimationEventHandler requires an EnemySpecialAttackSystem.");
            return;
        }

        if (behaviourFactory == null)
        {
            Debug.LogError(
                "EnemySpecialAttackAnimationEventHandler requires an EnemySpecialAttackBehaviourFactory.");
            return;
        }

        this.specialAttackSystem = specialAttackSystem;
        this.behaviourFactory = behaviourFactory;
    }

    public void OnSpecialAttackHit()
    {
        if (specialAttackSystem == null ||
            behaviourFactory == null ||
            !specialAttackSystem.IsAttackActive)
        {
            return;
        }

        EnemySpecialAttackDefinition definition =
            specialAttackSystem.ActiveAttack;

        if (definition == null)
            return;

        behaviourFactory.Execute(definition);
    }

    public void OnSpecialAttackRecoveryComplete()
    {
        if (specialAttackSystem == null ||
            !specialAttackSystem.IsAttackActive)
        {
            return;
        }

        specialAttackSystem.CompleteActiveAttack();
    }

    public void OnSpecialAttackInterrupted()
    {
        specialAttackSystem?.InterruptActiveAttack();
    }

    public void Clear()
    {
        specialAttackSystem = null;
        behaviourFactory = null;
    }
}
