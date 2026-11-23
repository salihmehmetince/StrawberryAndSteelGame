
using System.Collections.Generic;
using UnityEngine;

public class EnemySpecialAttackMeleeBehaviour : IEnemySpecialAttackBehaviour
{
    private readonly Transform attackOrigin;
    private readonly LayerMask targetLayers;

    public EnemySpecialAttackMeleeBehaviour(
        Transform attackOrigin,
        LayerMask targetLayers)
    {
        this.attackOrigin = attackOrigin;
        this.targetLayers = targetLayers;
    }

    public bool CanExecute(EnemySpecialAttackDefinition definition)
    {
        if (definition == null || definition.Damage <= 0 ||
            !IsFinite(definition.MeleeHitRadius) || definition.MeleeHitRadius <= 0f)
            return false;

        if (definition.AttackType != EnemySpecialAttackType.Melee)
            return false;

        if (attackOrigin == null)
            return false;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            definition.MeleeHitRadius,
            targetLayers);

        foreach (Collider2D target in targets)
        {
            if (target == null || target.transform.root == attackOrigin.root)
                continue;

            if (target.GetComponentInParent<IDamageable>() != null)
                return true;
        }

        return false;
    }

    public void Execute(EnemySpecialAttackDefinition definition)
    {
        if (definition == null || attackOrigin == null)
            return;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            definition.MeleeHitRadius,
            targetLayers);

        HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        foreach (Collider2D target in targets)
        {
            if (target == null || target.transform.root == attackOrigin.root)
                continue;

            IDamageable damageable =
                target.GetComponentInParent<IDamageable>();

            if (damageable == null ||
                !damagedTargets.Add(damageable))
            {
                continue;
            }

            damageable.ReceiveDamage(
                definition.Damage,
                attackOrigin.position);
        }
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
