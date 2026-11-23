
using UnityEngine;

public class EnemyAttackHitEventHandler : MonoBehaviour
{
    private EnemyAttackBehaviourSystem behaviourSystem;
    private EnemyAttackStateSystem stateSystem;
    private EnemyAttackSystem attackSystem;

    private EnemySpecialAttackSystem specialAttackSystem;
    private EnemySpecialAttackSequenceSystem specialSequenceSystem;
    private EnemySpecialAttackBehaviourFactory specialBehaviourFactory;

    public bool IsInitialized =>
        HasNormalAttackSystems ||
        HasSpecialAttackSystems;

    private bool HasNormalAttackSystems =>
        behaviourSystem != null &&
        stateSystem != null &&
        attackSystem != null &&
        behaviourSystem.IsInitialized &&
        attackSystem.IsInitialized;

    private bool HasSpecialAttackSystems =>
        specialAttackSystem != null &&
        specialSequenceSystem != null &&
        specialSequenceSystem.IsInitialized &&
        specialBehaviourFactory != null;

    public void Initialize(
        EnemyAttackBehaviourSystem behaviourSystem,
        EnemyAttackStateSystem stateSystem,
        EnemyAttackSystem attackSystem,
        EnemySpecialAttackSystem specialAttackSystem,
        EnemySpecialAttackSequenceSystem specialSequenceSystem,
        EnemySpecialAttackBehaviourFactory specialBehaviourFactory)
    {
        Clear();

        bool hasNormalSystems =
            behaviourSystem != null &&
            stateSystem != null &&
            attackSystem != null &&
            behaviourSystem.IsInitialized &&
            attackSystem.IsInitialized;

        bool hasSpecialSystems =
            specialAttackSystem != null &&
            specialSequenceSystem != null &&
            specialSequenceSystem.IsInitialized &&
            specialBehaviourFactory != null;

        if (!hasNormalSystems && !hasSpecialSystems)
        {
            Debug.LogError(
                "EnemyAttackHitEventHandler requires at least one complete attack system.",
                gameObject);

            return;
        }

        this.behaviourSystem = hasNormalSystems
            ? behaviourSystem
            : null;

        this.stateSystem = hasNormalSystems
            ? stateSystem
            : null;

        this.attackSystem = hasNormalSystems
            ? attackSystem
            : null;

        this.specialAttackSystem = hasSpecialSystems
            ? specialAttackSystem
            : null;

        this.specialSequenceSystem = hasSpecialSystems
            ? specialSequenceSystem
            : null;

        this.specialBehaviourFactory = hasSpecialSystems
            ? specialBehaviourFactory
            : null;
    }

    public void OnAttackHit()
    {
        if (!IsInitialized)
            return;

        if (specialAttackSystem != null &&
            specialAttackSystem.IsAttackActive)
        {
            if (specialSequenceSystem.CurrentState !=
                EnemyAttackState.Attacking)
            {
                return;
            }

            EnemySpecialAttackDefinition definition =
                specialAttackSystem.ActiveAttack;

            if (definition == null ||
                !specialBehaviourFactory.CanExecute(definition))
            {
                return;
            }

            specialBehaviourFactory.Execute(definition);
            return;
        }

        if (!HasNormalAttackSystems)
            return;

        if (!stateSystem.IsAttacking)
            return;

        if (!attackSystem.CanAttack())
            return;

        behaviourSystem.TryAttack();
    }

    public void Clear()
    {
        behaviourSystem = null;
        stateSystem = null;
        attackSystem = null;

        specialAttackSystem = null;
        specialSequenceSystem = null;
        specialBehaviourFactory = null;
    }
}
