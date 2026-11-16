
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyDefinition",
    menuName = "Strawberry And Steel/Enemies/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string enemyId;
    [SerializeField] private string enemyName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private GameObject enemyPrefab;

    [Header("Capabilities")]
    [SerializeField] private EnemyCapability capabilities;

    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float defense;
    [SerializeField] private int xpReward;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float acceleration;

    [Header("Detection")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float visionAngle;

    [Header("Combat")]
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;

    public string EnemyId => enemyId;
    public string EnemyName => enemyName;
    public string Description => description;
    public GameObject EnemyPrefab => enemyPrefab;

    public EnemyCapability Capabilities => capabilities;

    public int MaxHealth => maxHealth;
    public float Defense => defense;
    public int XPReward => xpReward;

    public float MovementSpeed => movementSpeed;
    public float Acceleration => acceleration;

    public float DetectionRange => detectionRange;
    public float VisionAngle => visionAngle;

    public float AttackDamage => attackDamage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    public bool HasCapability(EnemyCapability capability)
    {
        return (capabilities & capability) == capability;
    }
}
