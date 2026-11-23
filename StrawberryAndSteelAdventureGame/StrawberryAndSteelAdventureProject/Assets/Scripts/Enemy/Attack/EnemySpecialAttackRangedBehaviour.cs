
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
            definition.ProjectilePrefab == null)
        {
            return false;
        }

        return true;
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
}
