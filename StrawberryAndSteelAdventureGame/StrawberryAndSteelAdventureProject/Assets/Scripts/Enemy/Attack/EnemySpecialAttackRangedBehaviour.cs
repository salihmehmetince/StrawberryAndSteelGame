
using UnityEngine;

public class EnemySpecialAttackRangedBehaviour : IEnemySpecialAttackBehaviour
{
    private readonly EnemyAttackTargetSystem targetSystem;
    private readonly Transform projectileSpawnPoint;

    public EnemySpecialAttackRangedBehaviour(
        EnemyAttackTargetSystem targetSystem,
        Transform projectileSpawnPoint)
    {
        this.targetSystem = targetSystem;
        this.projectileSpawnPoint = projectileSpawnPoint;
    }

    public bool CanExecute(EnemySpecialAttackDefinition definition)
    {
        if (definition == null)
            return false;

        if (definition.AttackType != EnemySpecialAttackType.Ranged)
            return false;

        if (targetSystem == null ||
            projectileSpawnPoint == null ||
            targetSystem.CurrentTarget == null ||
            definition.ProjectilePrefab == null ||
            definition.Damage <= 0 ||
            !IsFinite(definition.ProjectileSpeed) || definition.ProjectileSpeed <= 0f ||
            !IsFinite(definition.ProjectileLifetime) || definition.ProjectileLifetime <= 0f)
        {
            return false;
        }

        Vector2 direction = (Vector2)(targetSystem.CurrentTarget.position - projectileSpawnPoint.position);
        return IsFinite(direction.x) && IsFinite(direction.y) && direction.sqrMagnitude > 0f;
    }

    public void Execute(EnemySpecialAttackDefinition definition)
    {
        if (!CanExecute(definition))
            return;

        Transform target = targetSystem.CurrentTarget;

        Vector2 direction =
            (target.position - projectileSpawnPoint.position).normalized;

        EnemyProjectile projectile = Object.Instantiate(
            definition.ProjectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity);

        if (projectile == null)
            return;

        projectile.Initialize(
            projectileSpawnPoint.root,
            definition.Damage,
            direction,
            definition.ProjectileSpeed,
            definition.ProjectileLifetime);

        if (!projectile.IsInitialized)
        {
            Object.Destroy(projectile.gameObject);
        }
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
