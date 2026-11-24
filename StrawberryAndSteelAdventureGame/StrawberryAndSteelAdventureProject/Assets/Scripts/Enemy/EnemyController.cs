
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Definition")]
    [SerializeField] private EnemyDefinition enemyDefinition;

    [Header("Enemy Components")]
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyDetection enemyDetection;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyCombat enemyCombat;
    [SerializeField] private EnemyDialogue enemyDialogue;

    private EnemyAnimationController enemyAnimationController;

    public EnemyDefinition Definition => enemyDefinition;

    public EnemyMovement Movement => enemyMovement;
    public EnemyDetection Detection => enemyDetection;
    public EnemyHealth Health => enemyHealth;
    public EnemyCombat Combat => enemyCombat;
    public EnemyDialogue Dialogue => enemyDialogue;

    public EnemyAnimationController Animation => enemyAnimationController;

    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

    private void Awake()
    {
        Animator animator = GetComponentInChildren<Animator>();

        if (animator != null)
            enemyAnimationController = new EnemyAnimationController(animator);
    }

    public void SetState(EnemyState state)
    {
        CurrentState = state;
    }
}
