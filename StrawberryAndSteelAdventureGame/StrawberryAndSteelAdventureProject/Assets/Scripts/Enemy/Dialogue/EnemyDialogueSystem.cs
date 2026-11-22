
using System.Collections.Generic;
using UnityEngine;

public class EnemyDialogueSystem
{
    private AudioSource audioSource;
    private EnemyDialogueDefinition definition;

    private float ambientTimer;
    private float dialogueCooldownTimer;

    private Dictionary<EnemyDialogueType, AudioClip> lastPlayedClips;

    private EnemyDialogueType activeDialogueType;
    private bool hasActiveDialogue;

    public bool IsInitialized { get; private set; }

    public bool IsPlaying =>
        audioSource != null && audioSource.isPlaying;

    public void Initialize(
        AudioSource audioSource,
        EnemyDialogueDefinition definition)
    {
        Clear();

        if (audioSource == null || definition == null)
            return;

        this.audioSource = audioSource;
        this.definition = definition;

        lastPlayedClips =
            new Dictionary<EnemyDialogueType, AudioClip>();

        ambientTimer = definition.AmbientCheckInterval;
        dialogueCooldownTimer = 0f;

        IsInitialized = true;
    }

    public void Update(float deltaTime, bool isIdle)
    {
        if (!IsInitialized)
            return;

        deltaTime = Mathf.Max(0f, deltaTime);

        if (dialogueCooldownTimer > 0f)
        {
            dialogueCooldownTimer =
                Mathf.Max(0f, dialogueCooldownTimer - deltaTime);
        }

        // Ambient yalnýzca Idle durumunda çalýþabilir.
        if (!isIdle)
        {
            if (hasActiveDialogue &&
                activeDialogueType == EnemyDialogueType.Ambient)
            {
                StopDialogue();
            }

            return;
        }

        if (IsPlaying)
            return;

        hasActiveDialogue = false;

        if (dialogueCooldownTimer > 0f)
            return;

        ambientTimer -= deltaTime;

        if (ambientTimer > 0f)
            return;

        ambientTimer = definition.AmbientCheckInterval;

        if (Random.value <= definition.AmbientPlayChance)
        {
            TryStartDialogue(EnemyDialogueType.Ambient);
        }
    }

    public bool PlayDialogue(EnemyDialogueType dialogueType)
    {
        if (!IsInitialized)
            return false;

        // Ambient yalnýzca Update üzerinden baþlatýlýr.
        if (dialogueType == EnemyDialogueType.Ambient)
            return false;

        return TryStartDialogue(dialogueType);
    }

    private bool TryStartDialogue(
        EnemyDialogueType dialogueType)
    {
        int newPriority = GetPriority(dialogueType);

        if (hasActiveDialogue && IsPlaying)
        {
            int currentPriority =
                GetPriority(activeDialogueType);

            // Ayný veya daha düþük öncelikli diyalog baþlatýlmaz.
            if (newPriority >= currentPriority)
                return false;

            // Daha yüksek öncelikli diyalog mevcut sesi keser.
            StopDialogue();
        }
        else
        {
            hasActiveDialogue = false;
        }

        AudioClip[] audioClips =
            definition.GetAudioClips(dialogueType);

        AudioClip selectedClip =
            SelectRandomClip(dialogueType, audioClips);

        if (selectedClip == null)
            return false;

        audioSource.PlayOneShot(selectedClip);

        activeDialogueType = dialogueType;
        hasActiveDialogue = true;

        dialogueCooldownTimer = definition.DialogueCooldown;

        return true;
    }

    private int GetPriority(EnemyDialogueType dialogueType)
    {
        switch (dialogueType)
        {
            case EnemyDialogueType.Death:
                return 1;

            case EnemyDialogueType.Hurt:
                return 2;

            case EnemyDialogueType.SpecialAttack:
                return 3;

            case EnemyDialogueType.Attack:
                return 4;

            case EnemyDialogueType.PlayerDetected:
                return 5;

            case EnemyDialogueType.LostTarget:
                return 6;

            case EnemyDialogueType.Ambient:
                return 7;

            default:
                return int.MaxValue;
        }
    }

    private AudioClip SelectRandomClip(
        EnemyDialogueType dialogueType,
        AudioClip[] audioClips)
    {
        if (audioClips == null || audioClips.Length == 0)
            return null;

        List<AudioClip> validClips = new List<AudioClip>();

        foreach (AudioClip clip in audioClips)
        {
            if (clip != null && !validClips.Contains(clip))
            {
                validClips.Add(clip);
            }
        }

        if (validClips.Count == 0)
            return null;

        if (validClips.Count == 1)
            return validClips[0];

        if (lastPlayedClips.TryGetValue(
            dialogueType,
            out AudioClip lastPlayedClip))
        {
            validClips.Remove(lastPlayedClip);
        }

        AudioClip selectedClip =
            validClips[Random.Range(0, validClips.Count)];

        lastPlayedClips[dialogueType] = selectedClip;

        return selectedClip;
    }

    public void StopDialogue()
    {
        if (audioSource != null)
            audioSource.Stop();

        hasActiveDialogue = false;
    }

    public void Clear()
    {
        if (audioSource != null)
            audioSource.Stop();

        audioSource = null;
        definition = null;

        ambientTimer = 0f;
        dialogueCooldownTimer = 0f;

        lastPlayedClips?.Clear();
        lastPlayedClips = null;

        hasActiveDialogue = false;
        IsInitialized = false;
    }
}
