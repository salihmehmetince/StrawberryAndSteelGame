using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cycles the direct Background children using a left/right TV-shutter transition.
/// </summary>
[DisallowMultipleComponent]
public sealed class MainMenuBackgroundTransition : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField, Min(0.1f)] private float transitionInterval = 25f;
    [SerializeField, Min(0.1f)] private float closeDuration = 0.65f;
    [SerializeField, Min(0.1f)] private float openDuration = 0.65f;
    [SerializeField, Min(0.1f)] private float beamCollapseDuration = 0.2f;
    [SerializeField, Min(1f)] private float beamMaximumWidth = 110f;

    private const float FullyClosedCoverage = 0.5f;
    private const float ClosedBeamWidth = 1f;

    private Canvas transitionCanvas;
    private RectTransform leftPanel;
    private RectTransform rightPanel;
    private RectTransform beamGlow;
    private RectTransform beamCore;
    private Sprite whiteSprite;
    private Texture2D whiteTexture;
    private int currentBackgroundIndex;

    private void Awake()
    {
        ResolveBackgrounds();
        if (backgrounds == null || backgrounds.Length < 2)
        {
            Debug.LogError("Backgrounds needs at least two child background objects.", this);
            enabled = false;
            return;
        }

        SelectCurrentBackground();
        CreateTransitionOverlay();
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, transitionInterval));
            int nextIndex = GetNextBackgroundIndex();
            yield return TransitionToBackground(nextIndex);
        }
    }

    private void ResolveBackgrounds()
    {
        if (backgrounds != null && backgrounds.Length > 0)
            return;

        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Background"))
                children.Add(child.gameObject);
        }

        backgrounds = children.ToArray();
    }

    private void SelectCurrentBackground()
    {
        currentBackgroundIndex = 0;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null && backgrounds[i].activeSelf)
            {
                currentBackgroundIndex = i;
                break;
            }
        }

        SetActiveBackground(currentBackgroundIndex);
    }

    private int GetNextBackgroundIndex()
    {
        int nextIndex = Random.Range(0, backgrounds.Length - 1);
        if (nextIndex >= currentBackgroundIndex)
            nextIndex++;
        return nextIndex;
    }

    private void SetActiveBackground(int activeIndex)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null)
                backgrounds[i].SetActive(i == activeIndex);
        }

        currentBackgroundIndex = activeIndex;
    }

    private IEnumerator TransitionToBackground(int nextIndex)
    {
        yield return AnimateCoverage(0f, FullyClosedCoverage, closeDuration);
        yield return AnimateBeam(0f, beamMaximumWidth, 0.08f);
        yield return AnimateBeam(beamMaximumWidth, ClosedBeamWidth, beamCollapseDuration);

        // Change the scene background while the screen is covered.
        SetActiveBackground(nextIndex);

        yield return AnimateBeam(ClosedBeamWidth, beamMaximumWidth, beamCollapseDuration);
        yield return AnimateOpen(openDuration);
    }

    private IEnumerator AnimateCoverage(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Ease(elapsed / duration);
            SetCoverage(Mathf.Lerp(from, to, progress));
            yield return null;
        }

        SetCoverage(to);
    }

    private IEnumerator AnimateBeam(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Ease(elapsed / duration);
            SetBeamWidth(Mathf.Lerp(from, to, progress));
            yield return null;
        }

        SetBeamWidth(to);
    }

    private IEnumerator AnimateOpen(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Ease(elapsed / duration);
            SetCoverage(Mathf.Lerp(FullyClosedCoverage, 0f, progress));
            SetBeamWidth(Mathf.Lerp(beamMaximumWidth, 0f, progress));
            yield return null;
        }

        SetCoverage(0f);
        SetBeamWidth(0f);
    }

    private static float Ease(float value)
    {
        float t = Mathf.Clamp01(value);
        return t * t * (3f - 2f * t);
    }

    private void CreateTransitionOverlay()
    {
        GameObject canvasObject = new GameObject(
            "BackgroundTransitionOverlay",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        transitionCanvas = canvasObject.GetComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.overrideSorting = true;
        transitionCanvas.sortingOrder = 32767;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            name = "BackgroundTransitionWhitePixel",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        whiteSprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        whiteSprite.name = "BackgroundTransitionWhiteSprite";
        whiteSprite.hideFlags = HideFlags.HideAndDontSave;

        leftPanel = CreateImage("LeftShutter", canvasObject.transform, Color.black).rectTransform;
        rightPanel = CreateImage("RightShutter", canvasObject.transform, Color.black).rectTransform;
        beamGlow = CreateImage(
            "CenterBeamGlow",
            canvasObject.transform,
            new Color(0.45f, 0.82f, 1f, 0.45f)).rectTransform;
        beamCore = CreateImage(
            "CenterBeam",
            canvasObject.transform,
            new Color(1f, 1f, 0.92f, 1f)).rectTransform;

        SetCoverage(0f);
        SetBeamWidth(0f);
    }

    private Image CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.GetComponent<Image>();
        image.sprite = whiteSprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private void SetCoverage(float coverage)
    {
        float amount = Mathf.Clamp(coverage, 0f, FullyClosedCoverage);

        leftPanel.anchorMin = Vector2.zero;
        leftPanel.anchorMax = new Vector2(amount, 1f);
        leftPanel.offsetMin = Vector2.zero;
        leftPanel.offsetMax = Vector2.zero;

        rightPanel.anchorMin = new Vector2(1f - amount, 0f);
        rightPanel.anchorMax = Vector2.one;
        rightPanel.offsetMin = Vector2.zero;
        rightPanel.offsetMax = Vector2.zero;
    }

    private void SetBeamWidth(float width)
    {
        float clampedWidth = Mathf.Max(0f, width);
        SetCenteredWidth(beamGlow, clampedWidth * 3f);
        SetCenteredWidth(beamCore, clampedWidth);
    }

    private static void SetCenteredWidth(RectTransform rectTransform, float width)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(width, 0f);
    }

    private void OnDestroy()
    {
        if (transitionCanvas != null)
            Destroy(transitionCanvas.gameObject);
        if (whiteSprite != null)
            Destroy(whiteSprite);
        if (whiteTexture != null)
            Destroy(whiteTexture);
    }
}
