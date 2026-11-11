
using SQLite;
using System.Linq;

public class PlayerCharacterStatsRepository
{
    private readonly SQLiteConnection database;

    public PlayerCharacterStatsRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerCharacterStats stats)
    {
        database.Insert(stats);
        return stats.PlayerCharacterStatsId;
    }

    public PlayerCharacterStats GetById(int statsId)
    {
        return database.Find<PlayerCharacterStats>(statsId);
    }

    public PlayerCharacterStats GetByPlayerId(int playerId)
    {
        return database.Query<PlayerCharacterStats>(
            "SELECT * FROM PlayerCharacterStats WHERE PlayerId = ?",
            playerId
        ).FirstOrDefault();
    }

    public bool Update(PlayerCharacterStats stats)
    {
        return database.Update(stats) > 0;
    }

    public bool Delete(int statsId)
    {
        return database.Delete<PlayerCharacterStats>(statsId) > 0;
    }

    public bool DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerCharacterStats WHERE PlayerId = ?",
            playerId
        ) > 0;
    }

    public bool Exists(int playerId)
    {
        return GetByPlayerId(playerId) != null;
    }
}
