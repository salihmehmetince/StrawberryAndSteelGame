
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthDisplaySystem
{
    private EnemyHealthSystem healthSystem;
    private Image healthBarFill;

    public bool IsInitialized => healthSystem != null;

    public void Initialize(
        EnemyHealthSystem healthSystem,
        Image healthBarFill)
    {
        if (healthSystem == null || healthBarFill == null)
        {
            Debug.LogError(
                "EnemyHealthDisplaySystem requires an EnemyHealthSystem and health bar Image.");

            return;
        }

        this.healthSystem = healthSystem;
        this.healthBarFill = healthBarFill;

        healthSystem.OnHealthChanged += UpdateHealthDisplay;

        UpdateHealthDisplay(
            healthSystem.CurrentHealth,
            healthSystem.MaxHealth);
    }

    private void UpdateHealthDisplay(
        int currentHealth,
        int maxHealth)
    {
        if (healthBarFill == null || maxHealth <= 0)
            return;

        healthBarFill.fillAmount =
            (float)currentHealth / maxHealth;
    }

    public void Dispose()
    {
        if (healthSystem == null)
            return;

        healthSystem.OnHealthChanged -= UpdateHealthDisplay;

        healthSystem = null;
        healthBarFill = null;
    }
}
