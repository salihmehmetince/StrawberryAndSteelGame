
using SQLite;

public class GameSettingsRepository
{
    private readonly SQLiteConnection database;

    private const int SettingsId = 1;

    public GameSettingsRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(GameSettings settings)
    {
        if (settings == null)
            return 0;

        settings.GameSettingsId = SettingsId;

        database.Insert(settings);

        return settings.GameSettingsId;
    }

    public GameSettings GetSettings()
    {
        return database.Find<GameSettings>(SettingsId);
    }

    public bool Update(GameSettings settings)
    {
        if (settings == null)
            return false;

        settings.GameSettingsId = SettingsId;

        return database.Update(settings) > 0;
    }

    public bool Delete()
    {
        return database.Delete<GameSettings>(SettingsId) > 0;
    }

    public bool Exists()
    {
        return database.Find<GameSettings>(SettingsId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM GameSettings"
        );
    }
}
