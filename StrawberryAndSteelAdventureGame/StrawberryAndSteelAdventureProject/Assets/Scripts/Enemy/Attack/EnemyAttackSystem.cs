
using UnityEngine;

public class EnemyAttackSystem
{
    private EnemyController enemyController;

    public bool IsInitialized =>
        enemyController != null;

    public void Initialize(EnemyController enemyController)
    {
        this.enemyController = null;

        if (enemyController == null)
        {
            Debug.LogError(
                "EnemyAttackSystem requires an EnemyController.");

            return;
        }

        this.enemyController = enemyController;
    }
    public bool CanAttack()
    {
        if (!IsInitialized || enemyController.IsDead)
            return false;

        return true;
    }
}
