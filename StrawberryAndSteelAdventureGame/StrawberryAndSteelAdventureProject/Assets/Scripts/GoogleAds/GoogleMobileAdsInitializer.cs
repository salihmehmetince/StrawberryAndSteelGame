
using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleMobileAdsInitializer : MonoBehaviour
{
    private BannerView bannerView;

    private const string TestAdUnitId =
        "ca-app-pub-3940256099942544/6300978111";

    private void Start()
    {
        MobileAds.Initialize(_ => LoadBanner());
    }

    private void LoadBanner()
    {
        int bannerWidth = 300;
        int bannerHeight = 250;

        int x = Mathf.RoundToInt(
            (Screen.width - bannerWidth) / 2f);

        int y = Mathf.RoundToInt(
            (Screen.height - bannerHeight) / 2f);

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
