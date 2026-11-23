
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemySpecialAttack",
    menuName = "Strawberry And Steel/Enemies/Special Attack Definition")]
public class EnemySpecialAttackDefinition : ScriptableObject
{
    [Header("General")]
    [SerializeField] private EnemySpecialAttackType attackType;
    [SerializeField] private int damage;
    [SerializeField] private float cooldown;

    [Header("Animation Timing")]
    [SerializeField] private float preparationDuration;
    [SerializeField] private float attackDuration;
    [SerializeField] private float recoveryDuration;

    [Header("Melee Configuration")]
    [SerializeField] private float meleeHitRadius;

    [Header("Ranged Configuration")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifetime;

    [Header("Area Configuration")]
    [SerializeField] private float areaRadius;

    public EnemySpecialAttackType AttackType => attackType;
    public int Damage => damage;
    public float Cooldown => cooldown;

    public float PreparationDuration => preparationDuration;
    public float AttackDuration => attackDuration;
    public float RecoveryDuration => recoveryDuration;

    public float MeleeHitRadius => meleeHitRadius;

    public EnemyProjectile ProjectilePrefab => projectilePrefab;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;

    public float AreaRadius => areaRadius;

    private void OnValidate()
    {
        if (!System.Enum.IsDefined(
            typeof(EnemySpecialAttackType),
            attackType))
        {
            attackType = EnemySpecialAttackType.None;
        }

        damage = Mathf.Max(0, damage);
        cooldown = Mathf.Max(0f, cooldown);

        preparationDuration =
            Mathf.Max(0f, preparationDuration);

        attackDuration =
            Mathf.Max(0.01f, attackDuration);

        recoveryDuration =
            Mathf.Max(0f, recoveryDuration);

        meleeHitRadius =
            Mathf.Max(0f, meleeHitRadius);

        projectileSpeed =
            Mathf.Max(0f, projectileSpeed);

        projectileLifetime =
            Mathf.Max(0.01f, projectileLifetime);

        areaRadius =
            Mathf.Max(0f, areaRadius);
    }
}
