
using UnityEngine;

public class EnemyAttackBehaviourFactory
{
    public IEnemyAttackBehaviour Create(
        EnemyAttackType attackType,
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackValidationSystem validationSystem,
        EnemyMeleeHitDetectionSystem hitDetectionSystem,
        Transform attackOrigin,
        float areaAttackRadius,
        LayerMask targetLayers,
        Transform projectileSpawnPoint,
        EnemyProjectile projectilePrefab,
        float projectileSpeed,
        float projectileLifetime)
    {
        switch (attackType)
        {
            case EnemyAttackType.None:
                return null;

            case EnemyAttackType.Melee:
                return CreateMeleeBehaviour(
                    attackSystem,
                    targetSystem,
                    damageSystem,
                    validationSystem,
                    hitDetectionSystem);

            case EnemyAttackType.Ranged:
                return CreateRangedBehaviour(
                    attackSystem,
                    targetSystem,
                    damageSystem,
                    validationSystem,
                    projectileSpawnPoint,
                    projectilePrefab,
                    projectileSpeed,
                    projectileLifetime);

            case EnemyAttackType.Area:
                return CreateAreaBehaviour(
                    attackSystem,
                    targetSystem,
                    damageSystem,
                    validationSystem,
                    attackOrigin,
                    areaAttackRadius,
                    targetLayers);

            default:
                Debug.LogError(
                    $"Unsupported enemy attack type: {attackType}");

                return null;
        }
    }

    private IEnemyAttackBehaviour CreateMeleeBehaviour(
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackValidationSystem validationSystem,
        EnemyMeleeHitDetectionSystem hitDetectionSystem)
    {
        EnemyMeleeAttackBehaviour behaviour =
            new EnemyMeleeAttackBehaviour();

        behaviour.Initialize(
            attackSystem,
            targetSystem,
            damageSystem,
            validationSystem,
            hitDetectionSystem);

        return behaviour.IsInitialized ? behaviour : null;
    }

    private IEnemyAttackBehaviour CreateRangedBehaviour(
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackValidationSystem validationSystem,
        Transform projectileSpawnPoint,
        EnemyProjectile projectilePrefab,
        float projectileSpeed,
        float projectileLifetime)
    {
        EnemyRangedAttackBehaviour behaviour =
            new EnemyRangedAttackBehaviour();

        behaviour.Initialize(
            attackSystem,
            targetSystem,
            damageSystem,
            validationSystem,
            projectileSpawnPoint,
            projectilePrefab,
            projectileSpeed,
            projectileLifetime);

        return behaviour.IsInitialized ? behaviour : null;
    }

    private IEnemyAttackBehaviour CreateAreaBehaviour(
        EnemyAttackSystem attackSystem,
        EnemyAttackTargetSystem targetSystem,
        EnemyAttackDamageSystem damageSystem,
        EnemyAttackValidationSystem validationSystem,
        Transform attackOrigin,
        float areaAttackRadius,
        LayerMask targetLayers)
    {
        EnemyAreaAttackBehaviour behaviour =
            new EnemyAreaAttackBehaviour();

        behaviour.Initialize(
            attackSystem,
            damageSystem,
            targetSystem,
            validationSystem,
            attackOrigin,
            areaAttackRadius,
            targetLayers);

        return behaviour.IsInitialized ? behaviour : null;
    }
}
