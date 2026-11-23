
using UnityEngine;

public class EnemyFeedbackSystem
{
    private AudioSource audioSource;
    private ParticleSystem hurtEffect;
    private ParticleSystem deathEffect;
    private ParticleSystem healEffect;

    private AudioClip hurtSound;
    private AudioClip deathSound;
    private AudioClip healSound;

    public bool IsInitialized { get; private set; }

    public void Initialize(
        AudioSource audioSource,
        AudioClip hurtSound,
        AudioClip deathSound,
        AudioClip healSound,
        ParticleSystem hurtEffect,
        ParticleSystem deathEffect,
        ParticleSystem healEffect)
    {
        Clear();

        this.audioSource = audioSource;
        this.hurtSound = hurtSound;
        this.deathSound = deathSound;
        this.healSound = healSound;
        this.hurtEffect = hurtEffect;
        this.deathEffect = deathEffect;
        this.healEffect = healEffect;

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

    public void PlayHealFeedback()
    {
        if (!IsInitialized)
            return;

        if (audioSource != null && healSound != null)
            audioSource.PlayOneShot(healSound);

        if (healEffect != null)
            healEffect.Play();
    }

    private void Clear()
    {
        audioSource = null;
        hurtEffect = null;
        deathEffect = null;
        healEffect = null;

        hurtSound = null;
        deathSound = null;
        healSound = null;

        IsInitialized = false;
    }
}
