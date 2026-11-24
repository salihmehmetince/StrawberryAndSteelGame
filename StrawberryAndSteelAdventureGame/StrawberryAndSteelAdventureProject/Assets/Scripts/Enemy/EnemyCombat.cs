
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyDefinition enemyDefinition;

    private PlayerCharacterSession playerSession;

    public void SetPlayerSession(PlayerCharacterSession session)
    {
        playerSession = session;
    }

    public void Attack()
    {
        if (playerSession == null || playerSession.IsDead)
            return;

        playerSession.TakeDamage(enemyDefinition.AttackDamage);
    }

    public void SpecialAttack(int damage)
    {
        if (playerSession == null || playerSession.IsDead || damage <= 0)
            return;

        playerSession.TakeDamage(damage);
    }
}
