using SQLite;

public class PlayerSettingsRepository
{
    private readonly SQLiteConnection database;

    public PlayerSettingsRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerSettings playerSettings)
    {
        database.Insert(playerSettings);
        return playerSettings.PlayerId;
    }

    public PlayerSettings GetByPlayerId(int playerId)
    {
        return database.Find<PlayerSettings>(playerId);
    }

    public bool Update(PlayerSettings playerSettings)
    {
        return database.Update(playerSettings) > 0;
    }

    public bool Delete(int playerId)
    {
        return database.Delete<PlayerSettings>(playerId) > 0;
    }

    public bool Exists(int playerId)
    {
        return GetByPlayerId(playerId) != null;
    }
}