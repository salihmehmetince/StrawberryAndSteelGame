
using System;
using SQLite;

public class MarketSettingsRepository
{
    private const int MarketSettingsId = 1;

    private readonly SQLiteConnection database;

    public MarketSettingsRepository(DatabaseContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        database = context.Connection;
    }

    public bool Add(MarketSettings settings)
    {
        if (settings == null)
            return false;

        settings.MarketSettingsId = MarketSettingsId;

        return database.Insert(settings) > 0;
    }

    public MarketSettings GetSettings()
    {
        return database.Find<MarketSettings>(MarketSettingsId);
    }

    public bool Update(MarketSettings settings)
    {
        if (settings == null)
            return false;

        settings.MarketSettingsId = MarketSettingsId;

        return database.Update(settings) > 0;
    }

    public bool Delete()
    {
        return database.Delete<MarketSettings>(MarketSettingsId) > 0;
    }

    public bool Exists()
    {
        return GetSettings() != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM MarketSettings"
        );
    }
}
