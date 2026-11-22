
using System;
using UnityEngine;

public class EnemyHealthEventHandler
{
    private EnemyHealthSystem healthSystem;
    private EnemyDamageSystem damageSystem;
    private EnemyHurtSystem hurtSystem;
    private EnemyDeathSystem deathSystem;
    private EnemyFeedbackSystem feedbackSystem;

    private Action<EnemyDialogueType> playDialogue;

    public bool IsInitialized =>
        healthSystem != null &&
        damageSystem != null &&
        hurtSystem != null &&
        deathSystem != null &&
        feedbackSystem != null;

    public void Initialize(
        EnemyHealthSystem healthSystem,
        EnemyDamageSystem damageSystem,
        EnemyHurtSystem hurtSystem,
        EnemyDeathSystem deathSystem,
        EnemyFeedbackSystem feedbackSystem,
        Action<EnemyDialogueType> playDialogue)
    {
        Dispose();

        if (healthSystem == null ||
            damageSystem == null ||
            hurtSystem == null ||
            deathSystem == null ||
            feedbackSystem == null)
        {
            Debug.LogError(
                "EnemyHealthEventHandler requires all systems.");

            return;
        }

        this.healthSystem = healthSystem;
        this.damageSystem = damageSystem;
        this.hurtSystem = hurtSystem;
        this.deathSystem = deathSystem;
        this.feedbackSystem = feedbackSystem;
        this.playDialogue = playDialogue;

        damageSystem.OnDamageReceived += HandleDamageReceived;

        healthSystem.OnHealed += HandleHealed;
        healthSystem.OnDied += HandleDied;
    }

    private void HandleDamageReceived(
        int damageAmount,
        Vector2 attackPosition)
    {
        if (!IsInitialized)
            return;

        if (healthSystem.IsDead || deathSystem.IsDead)
            return;

        hurtSystem.ApplyKnockback(attackPosition);
        feedbackSystem.PlayHurtFeedback();

        playDialogue?.Invoke(EnemyDialogueType.Hurt);
    }

    private void HandleHealed(int healAmount)
    {
        // Ýyileþtirme geri bildirimi sonraki aþamada baðlanacak.
    }

    private void HandleDied()
    {
        if (!IsInitialized)
            return;

        deathSystem.Die();
        feedbackSystem.PlayDeathFeedback();

        playDialogue?.Invoke(EnemyDialogueType.Death);
    }

    public void Dispose()
    {
        if (damageSystem != null)
            damageSystem.OnDamageReceived -= HandleDamageReceived;

        if (healthSystem != null)
        {
            healthSystem.OnHealed -= HandleHealed;
            healthSystem.OnDied -= HandleDied;
        }

        healthSystem = null;
        damageSystem = null;
        hurtSystem = null;
        deathSystem = null;
        feedbackSystem = null;

        playDialogue = null;
    }
}
