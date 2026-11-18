
using UnityEngine;

public class EnemyRangedAttackBehaviour : IEnemyAttackBehaviour
{
    private EnemyAttackSystem attackSystem;
    private EnemyAttackTargetSystem targetSystem;
    private EnemyAttackDamageSystem damageSystem;
    private EnemyAttackValidationSystem validationSystem;

    private Transform projectileSpawnPoint;
    private EnemyProjectile projectilePrefab;

    private float projectileSpeed;
    private float projectileLifetime;

    public bool IsInitialized =>
        attackSystem != null &&
        targetSystem != null &&
        damageSystem != null &&
        validationSystem != null &&
        projectileSpawnPoint != null &&
        projectilePrefab != null &&
        attackSystem.IsInitialized &&
        targetSystem.IsInitialized &&
        damageSystem.IsInitialized &&
        validationSystem.IsInitialized;

    public void Initialize(
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackValidationSystem validationSystem,
        Transform projectileSpawnPoint,
        EnemyProjectile projectilePrefab,
        float projectileSpeed,
        float projectileLifetime)
    {
        if (attackSystem == null ||
            targetSystem == null ||
            damageSystem == null ||
            validationSystem == null ||
            projectileSpawnPoint == null ||
            projectilePrefab == null ||
            projectileSpeed < 0f ||
            projectileLifetime <= 0f)
        {
            Debug.LogError(
                "EnemyRangedAttackBehaviour received invalid initialization values.");

            return;
        }

        if (!attackSystem.IsInitialized ||
            !targetSystem.IsInitialized ||
            !damageSystem.IsInitialized ||
            !validationSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyRangedAttackBehaviour received an uninitialized system.");

            return;
        }

        this.attackSystem = attackSystem;
        this.targetSystem = targetSystem;
        this.damageSystem = damageSystem;
        this.validationSystem = validationSystem;

        this.projectileSpawnPoint = projectileSpawnPoint;
        this.projectilePrefab = projectilePrefab;

        this.projectileSpeed = projectileSpeed;
        this.projectileLifetime = projectileLifetime;
    }

    public bool TryAttack()
    {
        if (!IsInitialized ||
            !validationSystem.CanAttack())
        {
            return false;
        }

        Transform target = targetSystem.CurrentTarget;

        Vector2 direction =
            (Vector2)(target.position - projectileSpawnPoint.position);

        if (direction.sqrMagnitude <= 0f)
            return false;

        EnemyProjectile projectile = Object.Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity);

        projectile.Initialize(
            projectileSpawnPoint.root,
            damageSystem.AttackDamage,
            direction,
            projectileSpeed,
            projectileLifetime);

        if (!projectile.IsInitialized)
        {
            Object.Destroy(projectile.gameObject);
            return false;
        }

        if (!attackSystem.TryStartAttack())
        {
            Object.Destroy(projectile.gameObject);
            return false;
        }

        return true;
    }
}
