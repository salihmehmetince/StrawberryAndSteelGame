
using System;

public class MarketSettingsService
{
    private readonly MarketSettingsRepository marketSettingsRepository;

    public MarketSettingsService(MarketSettingsRepository repository)
    {
        marketSettingsRepository = repository ??
            throw new ArgumentNullException(nameof(repository));
    }

    public bool CreateSettings(
        int baseCoinPrice,
        int baseDiamondPrice,
        int refreshIntervalMinutes = 30)
    {
        if (marketSettingsRepository.Exists())
            return false;

        if (baseCoinPrice < 0 ||
            baseDiamondPrice < 0 ||
            refreshIntervalMinutes <= 0)
            return false;

        MarketSettings settings = new MarketSettings
        {
            BaseCoinPrice = baseCoinPrice,
            BaseDiamondPrice = baseDiamondPrice,
            RefreshIntervalMinutes = refreshIntervalMinutes
        };

        return marketSettingsRepository.Add(settings);
    }

    public MarketSettings GetSettings()
    {
        return marketSettingsRepository.GetSettings();
    }

    public bool UpdateSettings(MarketSettings settings)
    {
        if (settings == null)
            return false;

        if (settings.BaseCoinPrice < 0 ||
            settings.BaseDiamondPrice < 0 ||
            settings.RefreshIntervalMinutes <= 0)
            return false;

        return marketSettingsRepository.Update(settings);
    }

    public bool DeleteSettings()
    {
        return marketSettingsRepository.Delete();
    }

    public bool SettingsExist()
    {
        return marketSettingsRepository.Exists();
    }

    public int GetSettingsCount()
    {
        return marketSettingsRepository.Count();
    }
}
