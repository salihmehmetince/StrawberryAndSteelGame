
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewProjectileDefinition",
    menuName = "Strawberry And Steel/Projectile/Projectile Definition"
)]
public class ProjectileDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string projectileID;
    [SerializeField] private string projectileName;
    [TextArea(3, 5)]
    [SerializeField] private string description;

    [Header("Player Effects")]
    [SerializeField] private float healthMultiplier = 1f;
    [SerializeField] private float manaMultiplier = 1f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float speedMultiplier = 1f;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float speedReductionPerSecond;

    [Header("Lifetime")]
    [SerializeField] private float lifetime;

    public string ProjectileID => projectileID;
    public string ProjectileName => projectileName;
    public string Description => description;

    public float HealthMultiplier => healthMultiplier;
    public float ManaMultiplier => manaMultiplier;
    public float DamageMultiplier => damageMultiplier;
    public float SpeedMultiplier => speedMultiplier;

    public float MovementSpeed => movementSpeed;
    public float SpeedReductionPerSecond => speedReductionPerSecond;

    public float Lifetime => lifetime;
}
