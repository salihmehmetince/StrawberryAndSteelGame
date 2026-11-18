
using UnityEngine;

public class EnemyAttackTimingSystem
{
    private float preparationDuration;
    private float attackDuration;
    private float recoveryDuration;

    private float remainingTime;

    public bool IsInitialized { get; private set; }

    public float RemainingTime => remainingTime;

    public float PreparationDuration => preparationDuration;
    public float AttackDuration => attackDuration;
    public float RecoveryDuration => recoveryDuration;

    public void Initialize(
        float preparationDuration,
        float attackDuration,
        float recoveryDuration)
    {
        if (preparationDuration < 0f ||
            attackDuration <= 0f ||
            recoveryDuration < 0f)
        {
            Debug.LogError(
                "EnemyAttackTimingSystem received invalid timing values.");

            return;
        }

        this.preparationDuration = preparationDuration;
        this.attackDuration = attackDuration;
        this.recoveryDuration = recoveryDuration;

        remainingTime = 0f;
        IsInitialized = true;
    }

    public void StartPreparation()
    {
        if (!IsInitialized)
            return;

        remainingTime = preparationDuration;
    }

    public void StartAttack()
    {
        if (!IsInitialized)
            return;

        remainingTime = attackDuration;
    }

    public void StartRecovery()
    {
        if (!IsInitialized)
            return;

        remainingTime = recoveryDuration;
    }

    public bool Update(float deltaTime)
    {
        if (!IsInitialized || remainingTime <= 0f)
            return true;

        remainingTime = Mathf.Max(
            0f,
            remainingTime - deltaTime);

        return remainingTime <= 0f;
    }

    public void Reset()
    {
        remainingTime = 0f;
    }
}
