
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LevelsSelectionBackgroundChain : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image chainImage;

    private Quaternion initialRotation;
    private Vector3 initialScale;
    private Color initialColor;

    private bool isPlaying;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        chainImage = GetComponent<Image>();

        initialRotation = rectTransform.localRotation;
        initialScale = rectTransform.localScale;
        initialColor = chainImage.color;
    }

    public void PlayBreakAnimation()
    {
        if (isPlaying)
            return;

        isPlaying = true;

        rectTransform.localRotation = initialRotation;
        rectTransform.localScale = initialScale;
        chainImage.color = initialColor;

        StartCoroutine(BreakAnimation());
    }


    private IEnumerator BreakAnimation()
    {
        // 1. Aþama: Yavaþça saða ve sola sallanma
        float shakeDuration = 2.5f;
        float shakeElapsed = 0f;

        float shakeAngle = Random.Range(10f, 20f);
        float shakeFrequency = 4f;

        while (shakeElapsed < shakeDuration)
        {
            shakeElapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(shakeElapsed / shakeDuration);
            float intensity = Mathf.Lerp(0.5f, 1f, progress);

            float angle =
                Mathf.Sin(shakeElapsed * shakeFrequency) *
                shakeAngle * intensity;

            rectTransform.localRotation =
                initialRotation * Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        // 2. Aþama: Yavaþça dönerek kýrýlma ve kaybolma
        float breakDuration = 1.2f;
        float breakElapsed = 0f;

        while (breakElapsed < breakDuration)
        {
            breakElapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(breakElapsed / breakDuration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            float rotation = Mathf.Lerp(0f, 100f, smoothProgress);

            rectTransform.localRotation =
                initialRotation * Quaternion.Euler(0f, 0f, rotation);

            rectTransform.localScale = Vector3.Lerp(
                initialScale,
                initialScale * 0.65f,
                smoothProgress
            );

            chainImage.color = new Color(
                initialColor.r,
                initialColor.g,
                initialColor.b,
                Mathf.Lerp(initialColor.a, 0f, smoothProgress)
            );

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
