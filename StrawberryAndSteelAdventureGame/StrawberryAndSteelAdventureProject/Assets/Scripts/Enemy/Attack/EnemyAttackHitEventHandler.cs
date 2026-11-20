
using UnityEngine;

public class EnemyAttackHitEventHandler : MonoBehaviour
{
    private EnemyAttackBehaviourSystem behaviourSystem;
    private EnemyAttackStateSystem stateSystem;
    private EnemyAttackSystem attackSystem;

    public bool IsInitialized =>
        behaviourSystem != null &&
        stateSystem != null &&
        attackSystem != null &&
        behaviourSystem.IsInitialized &&
        attackSystem.IsInitialized;

    public void Initialize(
        EnemyAttackBehaviourSystem behaviourSystem,
        EnemyAttackStateSystem stateSystem,
        EnemyAttackSystem attackSystem)
    {
        Clear();

        if (behaviourSystem == null ||
            stateSystem == null ||
            attackSystem == null)
        {
            Debug.LogError(
                "EnemyAttackHitEventHandler requires all attack systems.",
                gameObject);

            return;
        }

        if (!behaviourSystem.IsInitialized ||
            !attackSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackHitEventHandler received an uninitialized system.",
                gameObject);

            return;
        }

        this.behaviourSystem = behaviourSystem;
        this.stateSystem = stateSystem;
        this.attackSystem = attackSystem;
    }

    public void OnAttackHit()
    {
        if (!IsInitialized)
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
    }
}
