
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyDefinition enemyDefinition;
    [SerializeField] private Animator animator;

    private EnemyAnimationController animationController;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => enemyDefinition.MaxHealth;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (enemyDefinition == null)
        {
            Debug.LogError("EnemyDefinition is not assigned!", this);
            enabled = false;
            return;
        }

        CurrentHealth = enemyDefinition.MaxHealth;

        if (animator != null)
            animationController = new EnemyAnimationController(animator);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

        if (CurrentHealth == 0)
        {
            Die();
            return;
        }

        animationController?.PlayHurt();
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        animationController?.PlayDeath();
    }
}
