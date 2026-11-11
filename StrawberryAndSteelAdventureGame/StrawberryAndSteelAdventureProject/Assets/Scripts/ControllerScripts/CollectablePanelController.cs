
using System.Collections;
using TMPro;
using UnityEngine;

public class CollectablePanelController : MonoBehaviour
{
    public static CollectablePanelController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text diamondText;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float scaleMultiplier = 1.25f;

    private int previousCoinCount = -1;
    private int previousDiamondCount = -1;

    private Vector3 coinOriginalScale;
    private Vector3 diamondOriginalScale;

    private Coroutine coinAnimation;
    private Coroutine diamondAnimation;

    private void Awake()
    {
        Instance = this;

        if (coinText != null)
            coinOriginalScale = coinText.transform.localScale;

        if (diamondText != null)
            diamondOriginalScale = diamondText.transform.localScale;
    }

    private void Start()
    {
        RefreshCounts();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void RefreshCounts()
    {
        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper == null ||
            bootstrapper.LevelCollectableSession == null)
            return;

        int coinCount =
            bootstrapper.LevelCollectableSession.CoinCount;

        int diamondCount =
            bootstrapper.LevelCollectableSession.DiamondCount;

        UpdateCoinText(coinCount);
        UpdateDiamondText(diamondCount);
    }

    private void UpdateCoinText(int count)
    {
        if (coinText == null)
            return;

        coinText.text = count.ToString();

        if (previousCoinCount >= 0 && previousCoinCount != count)
        {
            PlayPunchAnimation(
                coinText.transform,
                coinOriginalScale,
                ref coinAnimation
            );
        }

        previousCoinCount = count;
    }

    private void UpdateDiamondText(int count)
    {
        if (diamondText == null)
            return;

        diamondText.text = count.ToString();

        if (previousDiamondCount >= 0 && previousDiamondCount != count)
        {
            PlayPunchAnimation(
                diamondText.transform,
                diamondOriginalScale,
                ref diamondAnimation
            );
        }

        previousDiamondCount = count;
    }

    private void PlayPunchAnimation(
        Transform target,
        Vector3 originalScale,
        ref Coroutine animationCoroutine)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        target.localScale = originalScale;

        animationCoroutine = StartCoroutine(
            PunchAnimation(target, originalScale)
        );
    }

    private IEnumerator PunchAnimation(
        Transform target,
        Vector3 originalScale)
    {
        Vector3 enlargedScale = originalScale * scaleMultiplier;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / animationDuration
            );

            float scaleValue = Mathf.Sin(progress * Mathf.PI);

            target.localScale = Vector3.Lerp(
                originalScale,
                enlargedScale,
                scaleValue
            );

            yield return null;
        }

        target.localScale = originalScale;
    }
}
