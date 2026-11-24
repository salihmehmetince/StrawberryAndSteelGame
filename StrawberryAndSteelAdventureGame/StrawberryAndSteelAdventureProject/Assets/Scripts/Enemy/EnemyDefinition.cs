
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyDefinition", menuName = "Strawberry And Steel/Enemy/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string enemyId;
    [SerializeField] private string enemyName;
    [TextArea(2, 5)]
    [SerializeField] private string description;

    [Header("Combat")]
    [SerializeField] private int attackDamage;
    [SerializeField] private float maxHealth;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;

    [Header("Reward")]
    [SerializeField] private int xpReward;

    public string EnemyId => enemyId;
    public string EnemyName => enemyName;
    public string Description => description;

    public int AttackDamage => attackDamage;
    public float MaxHealth => maxHealth;

    public float MovementSpeed => movementSpeed;

    public int XpReward => xpReward;
}
