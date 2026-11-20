
using UnityEngine;

public class EnemyFeedbackSystem
{
    private AudioSource audioSource;
    private ParticleSystem hurtEffect;
    private ParticleSystem deathEffect;

    private AudioClip hurtSound;
    private AudioClip deathSound;

    public bool IsInitialized { get; private set; }

    public void Initialize(
        AudioSource audioSource,
        AudioClip hurtSound,
        AudioClip deathSound,
        ParticleSystem hurtEffect,
        ParticleSystem deathEffect)
    {
        Clear();

        this.audioSource = audioSource;
        this.hurtSound = hurtSound;
        this.deathSound = deathSound;
        this.hurtEffect = hurtEffect;
        this.deathEffect = deathEffect;

        IsInitialized = true;
    }

    public void PlayHurtFeedback()
    {
        if (!IsInitialized)
            return;

        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        if (hurtEffect != null)
            hurtEffect.Play();
    }

    public void PlayDeathFeedback()
    {
        if (!IsInitialized)
            return;

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        if (deathEffect != null)
            deathEffect.Play();
    }

    private void Clear()
    {
        audioSource = null;
        hurtEffect = null;
        deathEffect = null;

        hurtSound = null;
        deathSound = null;

        IsInitialized = false;
    }
}
