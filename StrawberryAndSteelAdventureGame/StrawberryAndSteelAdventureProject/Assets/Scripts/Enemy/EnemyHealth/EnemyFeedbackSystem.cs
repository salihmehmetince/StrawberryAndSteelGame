
using UnityEngine;

public class EnemyFeedbackSystem
{
    private AudioSource audioSource;
    private ParticleSystem hurtEffect;
    private ParticleSystem deathEffect;

    private AudioClip hurtSound;
    private AudioClip deathSound;

    public bool IsInitialized =>
        audioSource != null ||
        hurtEffect != null ||
        deathEffect != null;

    public void Initialize(
        AudioSource audioSource,
        AudioClip hurtSound,
        AudioClip deathSound,
        ParticleSystem hurtEffect,
        ParticleSystem deathEffect)
    {
        this.audioSource = audioSource;
        this.hurtSound = hurtSound;
        this.deathSound = deathSound;
        this.hurtEffect = hurtEffect;
        this.deathEffect = deathEffect;
    }

    public void PlayHurtFeedback()
    {
        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        if (hurtEffect != null)
            hurtEffect.Play();
    }

    public void PlayDeathFeedback()
    {
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        if (deathEffect != null)
            deathEffect.Play();
    }
}
