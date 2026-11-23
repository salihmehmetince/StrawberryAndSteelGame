
using UnityEngine;

public class EnemyAttackAnimationEventHandler : MonoBehaviour
{
    private EnemyAttackSequenceSystem normalSequenceSystem;
    private EnemySpecialAttackSequenceSystem specialSequenceSystem;
    private EnemySpecialAttackSystem specialAttackSystem;

    public bool IsInitialized =>
        (normalSequenceSystem != null &&
         normalSequenceSystem.IsInitialized) ||
        (specialSequenceSystem != null &&
         specialSequenceSystem.IsInitialized &&
         specialAttackSystem != null);

    public void Initialize(
        EnemyAttackSequenceSystem normalSequenceSystem,
        EnemySpecialAttackSequenceSystem specialSequenceSystem,
        EnemySpecialAttackSystem specialAttackSystem)
    {
        Clear();

        bool hasNormalSequence =
            normalSequenceSystem != null &&
            normalSequenceSystem.IsInitialized;

        bool hasSpecialSequence =
            specialSequenceSystem != null &&
            specialSequenceSystem.IsInitialized &&
            specialAttackSystem != null;

        if (!hasNormalSequence && !hasSpecialSequence)
        {
            Debug.LogError(
                "EnemyAttackAnimationEventHandler requires at least one initialized attack sequence.",
                gameObject);

            return;
        }

        this.normalSequenceSystem = hasNormalSequence
            ? normalSequenceSystem
            : null;

        this.specialSequenceSystem = hasSpecialSequence
            ? specialSequenceSystem
            : null;

        this.specialAttackSystem = hasSpecialSequence
            ? specialAttackSystem
            : null;
    }

    private bool IsSpecialAttackActive =>
        specialAttackSystem != null &&
        specialAttackSystem.IsAttackActive;

    public void OnAttackPreparationComplete()
    {
        if (!IsInitialized)
            return;

        if (IsSpecialAttackActive)
        {
            specialSequenceSystem.StartAttackPhaseFromAnimation();
            return;
        }

        normalSequenceSystem?.StartAttackPhaseFromAnimation();
    }

    public void OnAttackAnimationComplete()
    {
        if (!IsInitialized)
            return;

        if (IsSpecialAttackActive)
        {
            specialSequenceSystem.StartRecoveryPhaseFromAnimation();
            return;
        }

        normalSequenceSystem?.StartRecoveryPhaseFromAnimation();
    }

    public void OnAttackRecoveryComplete()
    {
        if (!IsInitialized)
            return;

        if (IsSpecialAttackActive)
        {
            specialSequenceSystem.CompleteRecoveryFromAnimation();
            specialAttackSystem.CompleteActiveAttack();
            return;
        }

        normalSequenceSystem?.CompleteRecoveryFromAnimation();
    }

    public void OnAttackInterrupted()
    {
        if (!IsInitialized)
            return;

        if (IsSpecialAttackActive)
        {
            specialSequenceSystem.Interrupt();
            specialAttackSystem.InterruptActiveAttack();
            return;
        }

        normalSequenceSystem?.Interrupt();
    }

    public void Clear()
    {
        normalSequenceSystem = null;
        specialSequenceSystem = null;
        specialAttackSystem = null;
    }
}
