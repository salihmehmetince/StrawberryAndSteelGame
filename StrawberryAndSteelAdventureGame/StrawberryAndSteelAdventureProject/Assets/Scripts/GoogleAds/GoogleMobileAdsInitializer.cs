using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleMobileAdsInitializer : MonoBehaviour
{
    [SerializeField] private RectTransform adImage;

    private BannerView bannerView;

    private const string TestAdUnitId =
        "ca-app-pub-3940256099942544/6300978111";

    private void Start()
    {
        if (adImage == null)
        {
            Debug.LogError("AdImage referansý atanmamýþ.");
            return;
        }

        MobileAds.Initialize(_ => LoadBanner());
    }

    private void LoadBanner()
    {
        Canvas canvas = adImage.GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("AdImage bir Canvas altýnda bulunamadý.");
            return;
        }

        Vector3[] corners = new Vector3[4];
        adImage.GetWorldCorners(corners);

        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(
            uiCamera, corners[0]);

        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(
            uiCamera, corners[2]);

        int bannerWidth = 300;
        int bannerHeight = 250;

        int x = Mathf.RoundToInt(
            (bottomLeft.x + topRight.x - bannerWidth) / 2f);

        int y = Mathf.RoundToInt(
            Screen.height - (bottomLeft.y + topRight.y + bannerHeight) / 2f);

        bannerView?.Destroy();

        bannerView = new BannerView(
            TestAdUnitId,
            AdSize.MediumRectangle,
            Mathf.Max(0, x),
            Mathf.Max(0, y));

        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner reklamý yüklendi.");
        };

        bannerView.OnBannerAdLoadFailed += error =>
        {
            Debug.LogError("Banner yüklenemedi: " + error);
        };

        bannerView.LoadAd(new AdRequest());
    }

    private void OnDestroy()
    {
        bannerView?.Destroy();
        bannerView = null;
    }
}