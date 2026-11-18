
using UnityEngine;

public class EnemyAttackAnimationEventHandler : MonoBehaviour
{
    private EnemyAttackSequenceSystem sequenceSystem;

    public bool IsInitialized => sequenceSystem != null &&
                                 sequenceSystem.IsInitialized;

    public void Initialize(EnemyAttackSequenceSystem sequenceSystem)
    {
        if (sequenceSystem == null || !sequenceSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackAnimationEventHandler requires an initialized sequence system.",
                gameObject);

            return;
        }

        this.sequenceSystem = sequenceSystem;
    }

    public void OnAttackPreparationComplete()
    {
        if (!IsInitialized)
            return;

        sequenceSystem.StartAttackPhaseFromAnimation();
    }

    public void OnAttackAnimationComplete()
    {
        if (!IsInitialized)
            return;

        sequenceSystem.StartRecoveryPhaseFromAnimation();
    }

    public void OnAttackRecoveryComplete()
    {
        if (!IsInitialized)
            return;

        sequenceSystem.CompleteRecoveryFromAnimation();
    }

    public void OnAttackInterrupted()
    {
        if (!IsInitialized)
            return;

        sequenceSystem.Interrupt();
    }
}
