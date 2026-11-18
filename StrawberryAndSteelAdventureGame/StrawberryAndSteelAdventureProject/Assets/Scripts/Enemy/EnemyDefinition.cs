
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
    [SerializeField] private float deceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float airDeceleration;
    [SerializeField] private float turnDeceleration;
    [SerializeField] private float airTurnDeceleration;



    [Header("Detection")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float visionAngle;

    [Header("Combat")]
    [SerializeField] private EnemyAttackType attackType;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;


    [Header("Target Search")]
    [SerializeField] private float targetSearchSpeed;
    [SerializeField] private float targetSearchArrivalDistance;
    [SerializeField] private float targetSearchDuration;
    [SerializeField] private float targetSearchVerticalArrivalDistance;


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

    public float AirAcceleration => airAcceleration;


    public float Deceleration => deceleration;
    public float AirDeceleration => airDeceleration;

    public float TurnDeceleration => turnDeceleration;
    public float AirTurnDeceleration => airTurnDeceleration;


    public float TargetSearchSpeed => targetSearchSpeed;

    public float TargetSearchArrivalDistance =>
        targetSearchArrivalDistance;

    public float TargetSearchDuration => targetSearchDuration;

    public float TargetSearchVerticalArrivalDistance =>
    targetSearchVerticalArrivalDistance;

    public EnemyAttackType AttackType => attackType;
    public bool HasCapability(EnemyCapability capability)
    {
        return (capabilities & capability) == capability;
    }


    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        defense = Mathf.Max(0f, defense);
        xpReward = Mathf.Max(0, xpReward);

        movementSpeed = Mathf.Max(0f, movementSpeed);
        acceleration = Mathf.Max(0f, acceleration);
        airAcceleration = Mathf.Max(0f, airAcceleration);

        detectionRange = Mathf.Max(0f, detectionRange);
        visionAngle = Mathf.Clamp(visionAngle, 0f, 360f);

        attackDamage = Mathf.Max(0f, attackDamage);
        attackRange = Mathf.Max(0f, attackRange);
        attackCooldown = Mathf.Max(0f, attackCooldown);

        deceleration = Mathf.Max(0f, deceleration);
        airDeceleration = Mathf.Max(0f, airDeceleration);

        turnDeceleration = Mathf.Max(0f, turnDeceleration);
        airTurnDeceleration = Mathf.Max(0f, airTurnDeceleration);


        targetSearchSpeed = Mathf.Max(0.01f, targetSearchSpeed);

        targetSearchArrivalDistance =
            Mathf.Max(0f, targetSearchArrivalDistance);

        targetSearchDuration = Mathf.Max(0.01f, targetSearchDuration);
        targetSearchVerticalArrivalDistance =
    Mathf.Max(0f, targetSearchVerticalArrivalDistance);

        if (!System.Enum.IsDefined(typeof(EnemyAttackType), attackType))
        {
            attackType = EnemyAttackType.None;
        }

    }

}
