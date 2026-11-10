
using UnityEngine;

public class DiamondAnimationController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 240f;

    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.16f;
    [SerializeField] private float floatFrequency = 2.5f;

    [Header("Glow Settings")]
    [SerializeField] private SpriteRenderer glowRenderer;
    [SerializeField] private float glowMinAlpha = 0.2f;
    [SerializeField] private float glowMaxAlpha = 0.65f;
    [SerializeField] private float glowFrequency = 3f;

    [Header("Sparkle Settings")]
    [SerializeField] private ParticleSystem sparkleParticles;
    [SerializeField] private float sparkleInterval = 1.5f;
    [SerializeField] private int sparkleCount = 3;

    private Vector3 initialLocalPosition;
    private float sparkleTimer;
    private Color glowColor;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;

        if (glowRenderer != null)
            glowColor = glowRenderer.color;
    }

    private void Update()
    {
        RotateDiamond();
        FloatDiamond();
        AnimateGlow();
        PlaySparkles();
    }

    private void RotateDiamond()
    {
        transform.Rotate(
            0f,
            rotationSpeed * Time.deltaTime,
            0f,
            Space.Self
        );
    }

    private void FloatDiamond()
    {
        float offset = Mathf.Sin(Time.time * floatFrequency)
                       * floatAmplitude;

        Vector3 localPosition = initialLocalPosition;
        localPosition.y += offset;

        transform.localPosition = localPosition;
    }

    private void AnimateGlow()
    {
        if (glowRenderer == null)
            return;

        float glowValue = (Mathf.Sin(Time.time * glowFrequency) + 1f) / 2f;

        Color color = glowColor;
        color.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, glowValue);

        glowRenderer.color = color;
    }

    private void PlaySparkles()
    {
        if (sparkleParticles == null)
            return;

        sparkleTimer += Time.deltaTime;

        if (sparkleTimer >= sparkleInterval)
        {
            sparkleParticles.Emit(sparkleCount);
            sparkleTimer = 0f;
        }
    }
}
