using System.Collections.Generic;
using SQLite;

public class PlayerProgressRepository
{
    private readonly SQLiteConnection database;

    public PlayerProgressRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerProgress playerProgress)
    {
        database.Insert(playerProgress);
        return playerProgress.PlayerProgressId;
    }

    public PlayerProgress GetById(int playerProgressId)
    {
        return database.Find<PlayerProgress>(playerProgressId);
    }

    public PlayerProgress GetByPlayerId(int playerId)
    {
        List<PlayerProgress> results = database.Query<PlayerProgress>(
            "SELECT * FROM PlayerProgress WHERE PlayerId = ?",
            playerId
        );

        return results.Count > 0 ? results[0] : null;
    }

    public bool Update(PlayerProgress playerProgress)
    {
        return database.Update(playerProgress) > 0;
    }

    public bool Delete(int playerProgressId)
    {
        return database.Delete<PlayerProgress>(playerProgressId) > 0;
    }

    public bool DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerProgress WHERE PlayerId = ?",
            playerId
        ) > 0;
    }

    public bool Exists(int playerId)
    {
        return GetByPlayerId(playerId) != null;
    }
}