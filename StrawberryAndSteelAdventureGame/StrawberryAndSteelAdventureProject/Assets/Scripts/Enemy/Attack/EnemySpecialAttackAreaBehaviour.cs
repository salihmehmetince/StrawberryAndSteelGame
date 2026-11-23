
using System.Collections.Generic;
using UnityEngine;

public class EnemySpecialAttackAreaBehaviour : IEnemySpecialAttackBehaviour
{
    private readonly Transform attackOrigin;
    private readonly LayerMask targetLayers;

    public EnemySpecialAttackAreaBehaviour(
        Transform attackOrigin,
        LayerMask targetLayers)
    {
        this.attackOrigin = attackOrigin;
        this.targetLayers = targetLayers;
    }

    public bool CanExecute(EnemySpecialAttackDefinition definition)
    {
        if (definition == null)
            return false;

        if (definition.AttackType != EnemySpecialAttackType.Area)
            return false;

        if (attackOrigin == null)
            return false;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            definition.AreaRadius,
            targetLayers);

        return targets.Length > 0;
    }

    public void Execute(EnemySpecialAttackDefinition definition)
    {
        if (definition == null || attackOrigin == null)
            return;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            definition.AreaRadius,
            targetLayers);

        HashSet<IDamageable> damagedTargets =
            new HashSet<IDamageable>();

        foreach (Collider2D target in targets)
        {
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
}
