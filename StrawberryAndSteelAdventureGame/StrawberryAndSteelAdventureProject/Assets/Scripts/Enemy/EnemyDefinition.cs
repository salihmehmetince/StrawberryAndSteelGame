
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyDefinition",
    menuName = "Strawberry And Steel/Enemies/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    private const EnemyCapability ImplementedCapabilities =
        EnemyCapability.GroundMovement |
        EnemyCapability.Flying |
        EnemyCapability.Swimming |
        EnemyCapability.Jumping |
        EnemyCapability.Dashing |
        EnemyCapability.Charging |
        EnemyCapability.MeleeCombat |
        EnemyCapability.RangedCombat |
        EnemyCapability.AreaCombat;

    private const EnemyCapability ConfiguredCombatCapabilities =
        EnemyCapability.MeleeCombat |
        EnemyCapability.RangedCombat |
        EnemyCapability.AreaCombat;

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
    [SerializeField] private float attackPreparationDuration;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackRecoveryDuration;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifetime;

    [Header("Special Attacks")]
    [SerializeField] private EnemySpecialAttackDefinition[] specialAttacks;
    [SerializeField] private float specialAttackCheckIntervalMin;
    [SerializeField] private float specialAttackCheckIntervalMax;

    [Range(0f, 100f)]
    [SerializeField] private float specialAttackHealthThreshold;

    [Header("Target Search")]
    [SerializeField] private float targetSearchSpeed;
    [SerializeField] private float targetSearchArrivalDistance;
    [SerializeField] private float targetSearchDuration;
    [SerializeField] private float targetSearchVerticalArrivalDistance;

    [Header("Patrol")]
    [SerializeField] private EnemyPatrolSettings patrolSettings = new EnemyPatrolSettings();

    [Header("Retreat")]
    [SerializeField] private EnemyRetreatSettings retreatSettings = new EnemyRetreatSettings();

    [Header("Dash")]
    [SerializeField] private EnemyDashSettings dashSettings = new EnemyDashSettings();

    [Header("Charge")]
    [SerializeField] private EnemyChargeSettings chargeSettings = new EnemyChargeSettings();

    [Header("Dialogue")]
    [SerializeField] private EnemyDialogueDefinition dialogueDefinition;

    public string EnemyId => enemyId;
    public string EnemyName => enemyName;
    public string Description => description;
    public GameObject EnemyPrefab => enemyPrefab;

    /// <summary>Capabilities that have a working implementation in this code layer.</summary>
    public EnemyCapability Capabilities => GetEffectiveCapabilities();

    /// <summary>The serialized flags, including reserved capabilities not yet implemented.</summary>
    public EnemyCapability ConfiguredCapabilities => capabilities;

    public EnemyCapability UnsupportedCapabilities =>
        capabilities & ~ImplementedCapabilities;

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

    public EnemyPatrolSettings PatrolSettings => patrolSettings;
    public EnemyRetreatSettings RetreatSettings => retreatSettings;
    public EnemyDashSettings DashSettings => dashSettings;
    public EnemyChargeSettings ChargeSettings => chargeSettings;

    public EnemyAttackType AttackType => attackType;

    public float AttackPreparationDuration =>
        attackPreparationDuration;

    public float AttackDuration => attackDuration;

    public float AttackRecoveryDuration =>
        attackRecoveryDuration;

    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;

    public EnemySpecialAttackDefinition[] SpecialAttacks =>
        specialAttacks;

    public float SpecialAttackCheckIntervalMin =>
        specialAttackCheckIntervalMin;

    public float SpecialAttackCheckIntervalMax =>
        specialAttackCheckIntervalMax;

    public float SpecialAttackHealthThreshold =>
        specialAttackHealthThreshold;

    public EnemyDialogueDefinition DialogueDefinition =>
        dialogueDefinition;

    public bool HasCapability(EnemyCapability capability)
    {
        if (capability == EnemyCapability.None ||
            (capability & ~ImplementedCapabilities) != 0)
        {
            return capability == EnemyCapability.None;
        }

        return (GetEffectiveCapabilities() & capability) == capability;
    }

    private EnemyCapability GetEffectiveCapabilities()
    {
        EnemyCapability effective = capabilities &
            (EnemyCapability.GroundMovement |
             EnemyCapability.Flying |
             EnemyCapability.Swimming |
             EnemyCapability.Jumping);

        if (dashSettings != null && dashSettings.Enabled &&
            (capabilities & EnemyCapability.Dashing) != 0)
        {
            effective |= EnemyCapability.Dashing;
        }

        if (chargeSettings != null && chargeSettings.Enabled &&
            (capabilities & EnemyCapability.Charging) != 0)
        {
            effective |= EnemyCapability.Charging;
        }

        switch (attackType)
        {
            case EnemyAttackType.Melee:
                effective |= EnemyCapability.MeleeCombat;
                break;
            case EnemyAttackType.Ranged:
                effective |= EnemyCapability.RangedCombat;
                break;
            case EnemyAttackType.Area:
                effective |= EnemyCapability.AreaCombat;
                break;
        }

        return effective;
    }

    private void OnValidate()
    {
        if (patrolSettings == null)
            patrolSettings = new EnemyPatrolSettings();

        if (retreatSettings == null)
            retreatSettings = new EnemyRetreatSettings();

        if (dashSettings == null)
            dashSettings = new EnemyDashSettings();

        if (chargeSettings == null)
            chargeSettings = new EnemyChargeSettings();

        patrolSettings.Validate();
        retreatSettings.Validate();
        dashSettings.Validate();
        chargeSettings.Validate();

        maxHealth = Mathf.Max(1, maxHealth);
        defense = ClampFinite(defense, 0f, 0f);
        xpReward = Mathf.Max(0, xpReward);

        movementSpeed = ClampFinite(movementSpeed, 0f, 0f);
        acceleration = ClampFinite(acceleration, 0f, 0f);
        airAcceleration = ClampFinite(airAcceleration, 0f, 0f);

        detectionRange = ClampFinite(detectionRange, 0f, 0f);
        visionAngle = Mathf.Clamp(ClampFinite(visionAngle, 0f, 0f), 0f, 360f);

        attackDamage = ClampFinite(attackDamage, 0f, 0f);
        attackRange = ClampFinite(attackRange, 0f, 0f);
        attackCooldown = ClampFinite(attackCooldown, 0f, 0f);

        deceleration = ClampFinite(deceleration, 0f, 0f);
        airDeceleration = ClampFinite(airDeceleration, 0f, 0f);

        turnDeceleration = ClampFinite(turnDeceleration, 0f, 0f);
        airTurnDeceleration = ClampFinite(airTurnDeceleration, 0f, 0f);

        targetSearchSpeed = ClampFinite(targetSearchSpeed, 0.01f, 0.01f);

        targetSearchArrivalDistance =
            ClampFinite(targetSearchArrivalDistance, 0f, 0f);

        targetSearchDuration = ClampFinite(targetSearchDuration, 0.01f, 0.01f);

        targetSearchVerticalArrivalDistance =
            ClampFinite(targetSearchVerticalArrivalDistance, 0f, 0f);

        attackPreparationDuration =
            ClampFinite(attackPreparationDuration, 0f, 0f);

        attackDuration = ClampFinite(attackDuration, 0.01f, 0.01f);

        attackRecoveryDuration =
            ClampFinite(attackRecoveryDuration, 0f, 0f);

        projectileSpeed = ClampFinite(projectileSpeed, 0f, 0f);
        projectileLifetime = ClampFinite(projectileLifetime, 0.01f, 0.01f);

        specialAttackCheckIntervalMin =
            ClampFinite(specialAttackCheckIntervalMin, 0f, 0f);

        specialAttackCheckIntervalMax = Mathf.Max(
            specialAttackCheckIntervalMin,
            ClampFinite(specialAttackCheckIntervalMax, specialAttackCheckIntervalMin, 0f));

        specialAttackHealthThreshold =
            Mathf.Clamp(ClampFinite(specialAttackHealthThreshold, 0f, 0f), 0f, 100f);

        if (!System.Enum.IsDefined(typeof(EnemyAttackType), attackType))
        {
            attackType = EnemyAttackType.None;
        }

        // AttackType is the source of truth for implemented combat modes.
        // Keep legacy serialized bits intact, but do not expose mismatched bits as active.
        EnemyCapability configuredCombat = capabilities & ConfiguredCombatCapabilities;
        EnemyCapability expectedCombat = attackType switch
        {
            EnemyAttackType.Melee => EnemyCapability.MeleeCombat,
            EnemyAttackType.Ranged => EnemyCapability.RangedCombat,
            EnemyAttackType.Area => EnemyCapability.AreaCombat,
            _ => EnemyCapability.None
        };

        if (configuredCombat != EnemyCapability.None && configuredCombat != expectedCombat)
        {
            Debug.LogWarning(
                $"{name}: AttackType controls the active combat capability; serialized combat flags do not match it.",
                this);
        }
    }

    private static float ClampFinite(float value, float fallback, float minimum)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            return fallback;

        return Mathf.Max(minimum, value);
    }
}
