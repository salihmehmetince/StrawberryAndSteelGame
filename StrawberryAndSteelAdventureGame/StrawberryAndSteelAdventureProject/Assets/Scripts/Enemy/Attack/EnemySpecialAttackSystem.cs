using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpecialAttackSystem
{
    private readonly List<EnemySpecialAttackRuntime> attackRuntimes;
    private readonly List<EnemySpecialAttackRuntime> availableAttacks;
    private readonly float minimumCheckInterval;
    private readonly float maximumCheckInterval;
    private readonly float healthThreshold;
    private readonly Func<EnemySpecialAttackDefinition, bool> tryStartAttack;

    private float checkTimer;
    private float retryTimer;
    private bool healthThresholdReached;
    private bool healthThresholdPending;
    private bool timeCheckPending;

    public bool IsInitialized { get; private set; }
    public bool IsAttackActive { get; private set; }
    public bool IsAttackPending => healthThresholdPending || timeCheckPending;
    public EnemySpecialAttackDefinition ActiveAttack { get; private set; }

    public EnemySpecialAttackSystem(
        EnemySpecialAttackDefinition[] definitions,
        float minimumCheckInterval,
        float maximumCheckInterval,
        float healthThresholdPercentage,
        Func<EnemySpecialAttackDefinition, bool> tryStartAttack)
    {
        attackRuntimes = new List<EnemySpecialAttackRuntime>();
        availableAttacks = new List<EnemySpecialAttackRuntime>();
        this.tryStartAttack = tryStartAttack;

        if (!IsFinite(minimumCheckInterval) || !IsFinite(maximumCheckInterval) ||
            minimumCheckInterval < 0f || maximumCheckInterval < 0f)
        {
            Debug.LogError("EnemySpecialAttackSystem received invalid timing values.");
            minimumCheckInterval = 0.01f;
            maximumCheckInterval = 0.01f;
        }

        if (!IsFinite(healthThresholdPercentage))
            healthThresholdPercentage = 0f;

        this.minimumCheckInterval = Mathf.Max(0.01f, minimumCheckInterval);
        this.maximumCheckInterval = Mathf.Max(this.minimumCheckInterval, maximumCheckInterval);
        healthThreshold = Mathf.Clamp(healthThresholdPercentage / 100f, 0f, 1f);

        if (definitions != null)
        {
            foreach (EnemySpecialAttackDefinition definition in definitions)
            {
                if (definition == null || definition.AttackType == EnemySpecialAttackType.None)
                    continue;

                attackRuntimes.Add(new EnemySpecialAttackRuntime(definition));
            }
        }

        checkTimer = GetRandomCheckInterval();
        IsInitialized = attackRuntimes.Count > 0 && tryStartAttack != null;
    }

    public void Update(float deltaTime, float healthPercentage)
    {
        if (!IsInitialized || !IsFinite(deltaTime) || deltaTime < 0f)
            return;

        UpdateCooldowns(deltaTime);
        CheckHealthThreshold(healthPercentage);

        if (IsAttackActive)
            return;

        UpdateTimeTrigger(deltaTime);
        retryTimer = Mathf.Max(0f, retryTimer - deltaTime);
        if (retryTimer <= 0f)
            TryStartPendingAttack();
    }

    private void UpdateCooldowns(float deltaTime)
    {
        foreach (EnemySpecialAttackRuntime runtime in attackRuntimes)
            runtime.Update(deltaTime);
    }

    private void CheckHealthThreshold(float healthPercentage)
    {
        if (healthThresholdReached || !IsFinite(healthPercentage))
            return;

        if (healthPercentage > healthThreshold)
            return;

        healthThresholdReached = true;
        healthThresholdPending = true;
    }

    private void UpdateTimeTrigger(float deltaTime)
    {
        if (timeCheckPending)
            return;

        checkTimer -= deltaTime;
        if (checkTimer > 0f)
            return;

        timeCheckPending = true;
        checkTimer = GetRandomCheckInterval();
    }

    public bool TryStartPendingAttack()
    {
        if (!IsInitialized || IsAttackActive || !IsAttackPending || retryTimer > 0f)
            return false;

        if (!TryStartRandomAttack())
        {
            retryTimer = minimumCheckInterval;
            return false;
        }

        healthThresholdPending = false;
        timeCheckPending = false;
        retryTimer = 0f;
        return true;
    }

    private bool TryStartRandomAttack()
    {
        availableAttacks.Clear();
        foreach (EnemySpecialAttackRuntime runtime in attackRuntimes)
        {
            if (runtime.IsReady)
                availableAttacks.Add(runtime);
        }

        while (availableAttacks.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableAttacks.Count);
            EnemySpecialAttackRuntime selectedRuntime = availableAttacks[randomIndex];
            availableAttacks.RemoveAt(randomIndex);

            if (!tryStartAttack(selectedRuntime.Definition))
                continue;

            ActiveAttack = selectedRuntime.Definition;
            IsAttackActive = true;
            selectedRuntime.StartCooldown();
            return true;
        }

        return false;
    }

    public void CompleteActiveAttack()
    {
        if (!IsAttackActive)
            return;

        IsAttackActive = false;
        ActiveAttack = null;
    }

    public void InterruptActiveAttack()
    {
        if (!IsAttackActive)
            return;

        IsAttackActive = false;
        ActiveAttack = null;
    }

    private float GetRandomCheckInterval()
    {
        return UnityEngine.Random.Range(minimumCheckInterval, maximumCheckInterval);
    }

    public void Clear()
    {
        attackRuntimes.Clear();
        availableAttacks.Clear();
        IsAttackActive = false;
        ActiveAttack = null;
        healthThresholdReached = false;
        healthThresholdPending = false;
        timeCheckPending = false;
        checkTimer = 0f;
        retryTimer = 0f;
        IsInitialized = false;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
