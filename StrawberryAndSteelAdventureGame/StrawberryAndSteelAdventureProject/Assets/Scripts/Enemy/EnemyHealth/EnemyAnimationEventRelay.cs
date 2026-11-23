
using UnityEngine;

public class EnemyAnimationEventRelay : MonoBehaviour
{
    private EnemyController enemyController;

    public void Initialize(EnemyController enemyController)
    {
        this.enemyController = null;

        if (enemyController == null)
        {
            Debug.LogError(
                "EnemyAnimationEventRelay requires an EnemyController.");
            return;
        }

        this.enemyController = enemyController;
    }

    public void Clear()
    {
        enemyController = null;
    }

    public void CompleteHurtAnimation()
    {
        if (enemyController == null)
            return;

        enemyController.CompleteHurtAnimation();
    }

    public void CompleteDeathAnimation()
    {
        if (enemyController == null)
            return;

        enemyController.CompleteDeathAnimation();
    }
}
