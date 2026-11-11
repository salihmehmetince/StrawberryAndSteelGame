
using System.Collections.Generic;
using SQLite;

public class PlayerLevelCompletionRepository
{
    private readonly SQLiteConnection database;

    public PlayerLevelCompletionRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerLevelCompletion completion)
    {
        database.Insert(completion);
        return completion.PlayerLevelCompletionId;
    }

    public PlayerLevelCompletion GetById(int completionId)
    {
        return database.Find<PlayerLevelCompletion>(completionId);
    }

    public PlayerLevelCompletion GetByPlayerAndLevel(
        int playerId,
        int levelNumber)
    {
        List<PlayerLevelCompletion> results =
            database.Query<PlayerLevelCompletion>(
                "SELECT * FROM PlayerLevelCompletion WHERE PlayerId = ? AND LevelNumber = ?",
                playerId,
                levelNumber
            );

        return results.Count > 0 ? results[0] : null;
    }

    public List<PlayerLevelCompletion> GetByPlayerId(int playerId)
    {
        return database.Query<PlayerLevelCompletion>(
            "SELECT * FROM PlayerLevelCompletion WHERE PlayerId = ? ORDER BY LevelNumber",
            playerId
        );
    }

    public bool Update(PlayerLevelCompletion completion)
    {
        return database.Update(completion) > 0;
    }

    public bool Delete(int completionId)
    {
        return database.Delete<PlayerLevelCompletion>(completionId) > 0;
    }

    public int DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerLevelCompletion WHERE PlayerId = ?",
            playerId
        );
    }

    public bool Exists(int playerId, int levelNumber)
    {
        return GetByPlayerAndLevel(playerId, levelNumber) != null;
    }

    public int CountByPlayerId(int playerId)
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM PlayerLevelCompletion WHERE PlayerId = ?",
            playerId
        );
    }
}
