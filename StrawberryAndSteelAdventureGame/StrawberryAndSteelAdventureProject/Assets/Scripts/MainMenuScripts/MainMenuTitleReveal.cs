using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Curves the title around an upper circular arc and reveals it with a soft left-to-right sweep.
/// </summary>
[DisallowMultipleComponent]
public sealed class MainMenuTitleReveal : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float revealDuration = 1.7f;
    [SerializeField, Min(1f)] private float curveRadius = 1400f;
    [SerializeField, Min(1f)] private float revealFeatherWidth = 180f;
    [SerializeField] private Color leftTitleColor = new Color32(230, 183, 95, 255);
    [SerializeField] private Color centerTitleColor = new Color32(255, 240, 205, 255);
    [SerializeField] private Color rightTitleColor = new Color32(202, 217, 182, 255);

    private TextMeshProUGUI title;
    private Material titleMaterial;
    private float revealProgress;

    private void Awake()
    {
        title = GetComponent<TextMeshProUGUI>();
        if (title == null)
        {
            Debug.LogError("MainMenuTitleReveal needs a TextMeshProUGUI component.", this);
            enabled = false;
            return;
        }

        if (title.fontSharedMaterial != null)
        {
            titleMaterial = new Material(title.fontSharedMaterial)
            {
                name = "MainMenuTitleOutlinedMaterial"
            };
            if (titleMaterial.HasProperty("_OutlineWidth"))
                titleMaterial.SetFloat("_OutlineWidth", 0.12f);
            if (titleMaterial.HasProperty("_OutlineColor"))
                titleMaterial.SetColor("_OutlineColor", new Color(0.075f, 0.13f, 0.085f, 0.98f));
            title.fontMaterial = titleMaterial;
        }

        title.OnPreRenderText += ApplyCircularPaletteAndReveal;
        revealProgress = 0f;
    }

    private IEnumerator Start()
    {
        if (title == null)
            yield break;

        // Wait for Canvas layout so TMP has generated its complete character mesh.
        yield return null;
        title.ForceMeshUpdate();
        if (title.textInfo.characterCount == 0)
            yield break;

        float duration = Mathf.Max(0.1f, revealDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            revealProgress = Mathf.Clamp01(elapsed / duration);
            title.ForceMeshUpdate();
            yield return null;
        }

        revealProgress = 1f;
        title.ForceMeshUpdate();
    }

    private void ApplyCircularPaletteAndReveal(TMP_TextInfo textInfo)
    {
        if (textInfo == null || textInfo.characterInfo == null || textInfo.meshInfo == null)
            return;

        int characterCount = textInfo.characterCount;
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];
            if (!character.isVisible)
                continue;

            minX = Mathf.Min(minX, character.bottomLeft.x);
            maxX = Mathf.Max(maxX, character.topRight.x);
        }

        if (float.IsInfinity(minX) || float.IsInfinity(maxX))
            return;

        float textCenterX = (minX + maxX) * 0.5f;
        float halfSpan = Mathf.Max(1f, (maxX - minX) * 0.5f);
        float radius = Mathf.Max(curveRadius, halfSpan + 1f);
        float baseArcY = Mathf.Sqrt(Mathf.Max(0f, radius * radius - halfSpan * halfSpan));
        float sweepX = Mathf.Lerp(
            minX - revealFeatherWidth,
            maxX + revealFeatherWidth,
            revealProgress);

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];
            if (!character.isVisible)
                continue;

            int materialIndex = character.materialReferenceIndex;
            if (materialIndex < 0 || materialIndex >= textInfo.meshInfo.Length)
                continue;

            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            int firstVertex = character.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            if (vertexColors == null || vertices == null || firstVertex < 0
                || firstVertex + 3 >= vertexColors.Length || firstVertex + 3 >= vertices.Length)
                continue;

            float progress = characterCount <= 1 ? 0f : i / (float)(characterCount - 1);
            float centerX = (character.bottomLeft.x + character.topRight.x) * 0.5f;
            float centerY = (character.bottomLeft.y + character.topRight.y) * 0.5f;
            Color paletteColor = progress <= 0.5f
                ? Color.Lerp(leftTitleColor, centerTitleColor, progress * 2f)
                : Color.Lerp(centerTitleColor, rightTitleColor, (progress - 0.5f) * 2f);
            float relativeX = centerX - textCenterX;
            float circleRoot = Mathf.Sqrt(Mathf.Max(0.001f, radius * radius - relativeX * relativeX));
            float arcRise = circleRoot - baseArcY;
            float tangentAngle = Mathf.Atan2(-relativeX, circleRoot);
            float cosine = Mathf.Cos(tangentAngle);
            float sine = Mathf.Sin(tangentAngle);

            Vector3 pivot = new Vector3(centerX, centerY, 0f);
            Color32 color = paletteColor;
            for (int vertex = 0; vertex < 4; vertex++)
            {
                int vertexIndex = firstVertex + vertex;
                Vector3 originalPosition = vertices[vertexIndex];
                Vector3 offset = originalPosition - pivot;
                vertices[vertexIndex] = new Vector3(
                    centerX + offset.x * cosine - offset.y * sine,
                    centerY + arcRise + offset.x * sine + offset.y * cosine,
                    originalPosition.z);

                float alpha = Mathf.InverseLerp(
                    sweepX + revealFeatherWidth,
                    sweepX - revealFeatherWidth,
                    originalPosition.x);
                alpha = Mathf.SmoothStep(0f, 1f, alpha);
                color.a = (byte)Mathf.RoundToInt(alpha * 255f);
                vertexColors[vertexIndex] = color;
            }
        }
    }

    private void OnDestroy()
    {
        if (title != null)
            title.OnPreRenderText -= ApplyCircularPaletteAndReveal;

        if (titleMaterial != null)
            Destroy(titleMaterial);
    }
}
