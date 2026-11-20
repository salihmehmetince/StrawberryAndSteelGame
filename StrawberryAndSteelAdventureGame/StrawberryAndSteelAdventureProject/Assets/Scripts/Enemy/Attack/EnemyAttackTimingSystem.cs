
using UnityEngine;

public class EnemyAttackTimingSystem
{
    private float preparationDuration;
    private float attackDuration;
    private float recoveryDuration;

    private float remainingTime;

    public bool IsInitialized { get; private set; }
    public bool IsRunning { get; private set; }

    public float RemainingTime => remainingTime;

    public float PreparationDuration => preparationDuration;
    public float AttackDuration => attackDuration;
    public float RecoveryDuration => recoveryDuration;

    public void Initialize(
        float preparationDuration,
        float attackDuration,
        float recoveryDuration)
    {
        this.preparationDuration = 0f;
        this.attackDuration = 0f;
        this.recoveryDuration = 0f;

        remainingTime = 0f;
        IsRunning = false;
        IsInitialized = false;

        if (float.IsNaN(preparationDuration) ||
            float.IsInfinity(preparationDuration) ||
            preparationDuration < 0f ||
            float.IsNaN(attackDuration) ||
            float.IsInfinity(attackDuration) ||
            attackDuration <= 0f ||
            float.IsNaN(recoveryDuration) ||
            float.IsInfinity(recoveryDuration) ||
            recoveryDuration < 0f)
        {
            Debug.LogError(
                "Enemy attack timing values must be finite. Preparation and recovery must be non-negative, and attack duration must be greater than zero.");

            return;
        }

        this.preparationDuration = preparationDuration;
        this.attackDuration = attackDuration;
        this.recoveryDuration = recoveryDuration;

        IsInitialized = true;
    }

    public void StartPreparation()
    {
        StartTimer(preparationDuration);
    }

    public void StartAttack()
    {
        StartTimer(attackDuration);
    }

    public void StartRecovery()
    {
        StartTimer(recoveryDuration);
    }

    private void StartTimer(float duration)
    {
        if (!IsInitialized)
            return;

        remainingTime = duration;
        IsRunning = duration > 0f;
    }

    public bool Update(float deltaTime)
    {
        if (!IsInitialized ||
            !IsRunning ||
            float.IsNaN(deltaTime) ||
            float.IsInfinity(deltaTime) ||
            deltaTime < 0f)
        {
            return false;
        }

        remainingTime = Mathf.Max(
            0f,
            remainingTime - deltaTime);

        if (remainingTime > 0f)
            return false;

        IsRunning = false;
        return true;
    }
    public void Reset()
    {
        remainingTime = 0f;
        IsRunning = false;
    }
}
