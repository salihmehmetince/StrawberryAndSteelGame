
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStateAudioDefinition audioDefinition;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Speak(EnemyState state)
    {
        Speak(state, false);
    }

    public void Speak(EnemyState state, bool interruptCurrent)
    {
        if (audioDefinition == null)
            return;

        AudioClip dialogueClip = GetDialogueClip(state);
        if (dialogueClip == null)
            dialogueClip = audioDefinition.DefaultAudio;

        if (dialogueClip == null)
            return;

        if (audioSource.isPlaying)
        {
            if (!interruptCurrent)
                return;

            audioSource.Stop();
        }

        audioSource.PlayOneShot(dialogueClip);
    }

    public void StopSpeaking()
    {
        audioSource.Stop();
    }

    private AudioClip GetDialogueClip(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                return audioDefinition.IdleAudio;

            case EnemyState.Patrol:
                return audioDefinition.PatrolAudio;

            case EnemyState.Chase:
                return audioDefinition.ChaseAudio;

            case EnemyState.Attack:
                return audioDefinition.AttackAudio;

            case EnemyState.SpecialAttack:
                return audioDefinition.SpecialAttackAudio;

            case EnemyState.Hurt:
                return audioDefinition.HurtAudio;

            case EnemyState.Death:
                return audioDefinition.DeathAudio;

            case EnemyState.Heal:
                return audioDefinition.HealAudio;

            default:
                return audioDefinition.DefaultAudio;
        }
    }
}
