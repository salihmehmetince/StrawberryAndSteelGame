
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
        cooldown = ClampFinite(cooldown, 0f, 0f);

        preparationDuration =
            ClampFinite(preparationDuration, 0f, 0f);

        attackDuration =
            ClampFinite(attackDuration, 0.01f, 0.01f);

        recoveryDuration =
            ClampFinite(recoveryDuration, 0f, 0f);

        meleeHitRadius =
            ClampFinite(meleeHitRadius, 0f, 0f);

        projectileSpeed =
            ClampFinite(projectileSpeed, 0f, 0f);

        projectileLifetime =
            ClampFinite(projectileLifetime, 0.01f, 0.01f);

        areaRadius =
            ClampFinite(areaRadius, 0f, 0f);
    }

    private static float ClampFinite(float value, float fallback, float minimum)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            return fallback;

        return Mathf.Max(minimum, value);
    }
}
