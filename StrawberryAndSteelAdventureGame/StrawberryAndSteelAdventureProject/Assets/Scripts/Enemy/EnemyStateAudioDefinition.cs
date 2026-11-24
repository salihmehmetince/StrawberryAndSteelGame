
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyStateAudioDefinition",
    menuName = "Strawberry And Steel/Enemy/Enemy State Audio Definition"
)]
public class EnemyStateAudioDefinition : ScriptableObject
{
    [Header("Default Audio")]
    [SerializeField] private AudioClip defaultAudio;

    [Header("Enemy State Audio Clips")]
    [SerializeField] private AudioClip idleAudio;
    [SerializeField] private AudioClip patrolAudio;
    [SerializeField] private AudioClip chaseAudio;
    [SerializeField] private AudioClip attackAudio;
    [SerializeField] private AudioClip specialAttackAudio;
    [SerializeField] private AudioClip hurtAudio;
    [SerializeField] private AudioClip deathAudio;
    [SerializeField] private AudioClip healAudio;

    public AudioClip DefaultAudio => defaultAudio;

    public AudioClip IdleAudio => idleAudio;
    public AudioClip PatrolAudio => patrolAudio;
    public AudioClip ChaseAudio => chaseAudio;
    public AudioClip AttackAudio => attackAudio;
    public AudioClip SpecialAttackAudio => specialAttackAudio;
    public AudioClip HurtAudio => hurtAudio;
    public AudioClip DeathAudio => deathAudio;
    public AudioClip HealAudio => healAudio;
}
