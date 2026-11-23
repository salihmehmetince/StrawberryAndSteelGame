
using UnityEngine;

/// <summary>
/// Legacy special-only event bridge. EnemyController now routes both normal and
/// special attack events through EnemyAttackAnimationEventHandler and
/// EnemyAttackHitEventHandler; this component is not initialized by the controller.
/// </summary>
public class EnemySpecialAttackAnimationEventHandler : MonoBehaviour
{
    private EnemySpecialAttackSystem specialAttackSystem;
    private EnemySpecialAttackBehaviourFactory behaviourFactory;

    public bool IsInitialized => specialAttackSystem != null && behaviourFactory != null;

    public void Initialize(
        EnemySpecialAttackSystem specialAttackSystem,
        EnemySpecialAttackBehaviourFactory behaviourFactory)
    {
        Clear();

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
        if (!IsInitialized ||
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
        if (!IsInitialized || !specialAttackSystem.IsAttackActive)
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
