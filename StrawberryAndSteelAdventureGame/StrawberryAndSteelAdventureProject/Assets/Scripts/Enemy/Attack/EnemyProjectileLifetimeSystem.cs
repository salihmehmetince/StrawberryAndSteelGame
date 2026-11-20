using UnityEngine;

public class EnemyProjectileLifetimeSystem
{
    private float lifetime;
    private float remainingLifetime;

    public bool IsInitialized { get; private set; }

    public bool IsExpired =>
        IsInitialized && remainingLifetime <= 0f;

    public float RemainingLifetime => remainingLifetime;

    public void Initialize(float lifetime)
    {
        this.lifetime = 0f;
        remainingLifetime = 0f;
        IsInitialized = false;

        if (float.IsNaN(lifetime) ||
            float.IsInfinity(lifetime) ||
            lifetime <= 0f)
        {
            Debug.LogError(
                "EnemyProjectileLifetimeSystem received an invalid lifetime.");

            return;
        }

        this.lifetime = lifetime;
        remainingLifetime = lifetime;

        IsInitialized = true;
    }

    public void Update(float deltaTime)
    {
        if (!IsInitialized ||
            IsExpired ||
            float.IsNaN(deltaTime) ||
            float.IsInfinity(deltaTime) ||
            deltaTime < 0f)
        {
            return;
        }

        remainingLifetime = Mathf.Max(
            0f,
            remainingLifetime - deltaTime);
    }

    public void ResetLifetime()
    {
        if (!IsInitialized)
            return;

        remainingLifetime = lifetime;
    }
}