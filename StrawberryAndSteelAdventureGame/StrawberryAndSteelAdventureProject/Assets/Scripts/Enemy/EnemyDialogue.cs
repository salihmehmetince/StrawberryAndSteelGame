
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStateAudioDefinition audioDefinition;
    [SerializeField] private bool debugLogs = true;

    private AudioSource audioSource;
    private AudioClip currentDialogueClip;

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
        {
            if (debugLogs)
                Debug.LogWarning($"[EnemyDialogue] State={state}; no audio definition assigned.", this);
            return;
        }

        AudioClip dialogueClip = GetDialogueClip(state);
        if (dialogueClip == null)
            dialogueClip = audioDefinition.DefaultAudio;

        if (dialogueClip == null)
        {
            if (debugLogs)
                Debug.Log($"[EnemyDialogue] State={state}; no audio clip configured.", this);
            return;
        }

        if (audioSource.isPlaying)
        {
            if (!interruptCurrent)
            {
                if (debugLogs)
                {
                    string playingName = currentDialogueClip != null
                        ? currentDialogueClip.name
                        : "unknown";
                    Debug.Log($"[EnemyDialogue] State={state}; keeping current SFX='{playingName}'.", this);
                }
                return;
            }

            if (debugLogs)
            {
                string playingName = currentDialogueClip != null
                    ? currentDialogueClip.name
                    : "unknown";
                Debug.Log($"[EnemyDialogue] interrupt SFX='{playingName}' for State={state}.", this);
            }
            audioSource.Stop();
        }

        audioSource.PlayOneShot(dialogueClip);
        currentDialogueClip = dialogueClip;

        if (debugLogs)
            Debug.Log($"[EnemyDialogue] State={state}; SFX='{dialogueClip.name}' started.", this);
    }

    public void StopSpeaking()
    {
        if (audioSource.isPlaying && debugLogs)
        {
            string playingName = currentDialogueClip != null
                ? currentDialogueClip.name
                : "unknown";
            Debug.Log($"[EnemyDialogue] SFX='{playingName}' stopped.", this);
        }

        audioSource.Stop();
        currentDialogueClip = null;
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
