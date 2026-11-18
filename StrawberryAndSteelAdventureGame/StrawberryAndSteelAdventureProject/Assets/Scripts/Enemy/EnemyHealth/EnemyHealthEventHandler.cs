
using UnityEngine;

public class EnemyHealthEventHandler
{
    private EnemyHealthSystem healthSystem;
    private EnemyDamageSystem damageSystem;
    private EnemyHurtSystem hurtSystem;
    private EnemyDeathSystem deathSystem;
    private EnemyFeedbackSystem feedbackSystem;

    public bool IsInitialized => healthSystem != null;

    public void Initialize(
        EnemyHealthSystem healthSystem,
        EnemyDamageSystem damageSystem,
        EnemyHurtSystem hurtSystem,
        EnemyDeathSystem deathSystem,
        EnemyFeedbackSystem feedbackSystem)
    {
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

        damageSystem.OnDamageReceived += HandleDamageReceived;

        healthSystem.OnHealed += HandleHealed;
        healthSystem.OnDied += HandleDied;
    }

    private void HandleDamageReceived(
        int damageAmount,
        Vector2 attackPosition)
    {
        if (healthSystem.IsDead || deathSystem.IsDead)
            return;

        hurtSystem.ApplyKnockback(attackPosition);
        feedbackSystem.PlayHurtFeedback();
    }

    private void HandleHealed(int healAmount)
    {
        // Ýyileþtirme geri bildirimini sonraki adýmda baðlayacaðýz.
    }

    private void HandleDied()
    {
        deathSystem.Die();
        feedbackSystem.PlayDeathFeedback();
    }

    public void Dispose()
    {
        if (healthSystem == null)
            return;

        damageSystem.OnDamageReceived -= HandleDamageReceived;

        healthSystem.OnHealed -= HandleHealed;
        healthSystem.OnDied -= HandleDied;

        healthSystem = null;
        damageSystem = null;
        hurtSystem = null;
        deathSystem = null;
        feedbackSystem = null;
    }
}
