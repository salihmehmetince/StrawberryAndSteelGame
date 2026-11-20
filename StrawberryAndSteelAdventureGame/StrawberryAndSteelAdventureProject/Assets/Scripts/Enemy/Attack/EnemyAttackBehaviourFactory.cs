
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
        if (attackSystem == null ||
            !attackSystem.IsInitialized ||
            targetSystem == null ||
            !targetSystem.IsInitialized ||
            damageSystem == null ||
            !damageSystem.IsInitialized ||
            validationSystem == null ||
            !validationSystem.IsInitialized)
        {
            Debug.LogError(
                "EnemyAttackBehaviourFactory received invalid common attack systems.");

            return null;
        }

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
        if (hitDetectionSystem == null ||
            !hitDetectionSystem.IsInitialized)
        {
            Debug.LogError(
                "Melee attack requires an initialized hit detection system.");

            return null;
        }

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
        if (projectileSpawnPoint == null ||
            projectilePrefab == null ||
            !IsValidPositiveValue(projectileSpeed) ||
            !IsValidPositiveValue(projectileLifetime))
        {
            Debug.LogError(
                "Ranged attack received invalid projectile configuration.");

            return null;
        }

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
        if (attackOrigin == null ||
            !IsValidPositiveValue(areaAttackRadius))
        {
            Debug.LogError(
                "Area attack received invalid attack origin or radius.");

            return null;
        }

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

    private bool IsValidPositiveValue(float value)
    {
        return !float.IsNaN(value) &&
               !float.IsInfinity(value) &&
               value > 0f;
    }
}
