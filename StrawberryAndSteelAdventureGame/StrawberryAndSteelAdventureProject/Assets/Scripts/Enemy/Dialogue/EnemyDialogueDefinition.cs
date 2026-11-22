
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyDialogueGroup
{
    [SerializeField] private EnemyDialogueType dialogueType;
    [SerializeField] private AudioClip[] audioClips;

    public EnemyDialogueType DialogueType => dialogueType;
    public AudioClip[] AudioClips => audioClips;
}

[CreateAssetMenu(
    fileName = "NewEnemyDialogueDefinition",
    menuName = "Strawberry And Steel/Enemies/Enemy Dialogue Definition")]
public class EnemyDialogueDefinition : ScriptableObject
{
    [Header("Dialogue Groups")]
    [SerializeField] private EnemyDialogueGroup[] dialogueGroups;

    [Header("Ambient Dialogue")]
    [SerializeField] private float ambientCheckInterval;
    [SerializeField, Range(0f, 1f)]
    private float ambientPlayChance;

    [Header("Dialogue Settings")]
    [SerializeField] private float dialogueCooldown;

    [Header("Default Dialogue")]
    [SerializeField] private AudioClip defaultDialogueClip;
    public float AmbientCheckInterval => ambientCheckInterval;
    public float AmbientPlayChance => ambientPlayChance;
    public float DialogueCooldown => dialogueCooldown;

    public AudioClip[] GetAudioClips(EnemyDialogueType dialogueType)
    {
        List<AudioClip> audioClips = new List<AudioClip>();

        if (dialogueGroups != null)
        {
            foreach (EnemyDialogueGroup group in dialogueGroups)
            {
                if (group == null || group.DialogueType != dialogueType)
                    continue;

                if (group.AudioClips == null)
                    continue;

                foreach (AudioClip clip in group.AudioClips)
                {
                    if (clip != null && !audioClips.Contains(clip))
                        audioClips.Add(clip);
                }
            }
        }

        if (audioClips.Count == 0 && defaultDialogueClip != null)
            return new AudioClip[] { defaultDialogueClip };

        return audioClips.ToArray();
    }
    private void OnValidate()
    {
        ambientCheckInterval = Mathf.Max(0.1f, ambientCheckInterval);
        ambientPlayChance = Mathf.Clamp01(ambientPlayChance);
        dialogueCooldown = Mathf.Max(0f, dialogueCooldown);
    }
}
