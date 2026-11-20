
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
        this.attackSystem = null;
        this.targetSystem = null;
        this.damageSystem = null;
        this.validationSystem = null;

        this.projectileSpawnPoint = null;
        this.projectilePrefab = null;

        this.projectileSpeed = 0f;
        this.projectileLifetime = 0f;

        if (attackSystem == null ||
            targetSystem == null ||
            damageSystem == null ||
            validationSystem == null ||
            projectileSpawnPoint == null ||
            projectilePrefab == null ||
            float.IsNaN(projectileSpeed) ||
            float.IsInfinity(projectileSpeed) ||
            projectileSpeed <= 0f ||
            float.IsNaN(projectileLifetime) ||
            float.IsInfinity(projectileLifetime) ||
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

        if (target == null)
            return false;

        Vector2 direction =
            (Vector2)(target.position - projectileSpawnPoint.position);

        if (float.IsNaN(direction.x) ||
            float.IsInfinity(direction.x) ||
            float.IsNaN(direction.y) ||
            float.IsInfinity(direction.y) ||
            direction.sqrMagnitude <= Mathf.Epsilon ||
            float.IsInfinity(direction.sqrMagnitude))
        {
            return false;
        }

        direction.Normalize();

        EnemyProjectile projectile = Object.Instantiate(
            projectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity);

        if (projectile == null)
            return false;

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

        return true;
    }
}
