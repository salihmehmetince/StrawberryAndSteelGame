
using System.Collections.Generic;
using SQLite;

public class PlayerActiveSpellRepository
{
    private readonly SQLiteConnection database;

    public PlayerActiveSpellRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerActiveSpell playerActiveSpell)
    {
        database.Insert(playerActiveSpell);
        return playerActiveSpell.PlayerActiveSpellId;
    }

    public PlayerActiveSpell GetById(int playerActiveSpellId)
    {
        return database.Find<PlayerActiveSpell>(playerActiveSpellId);
    }

    public List<PlayerActiveSpell> GetByPlayerId(int playerId)
    {
        return database.Query<PlayerActiveSpell>(
            "SELECT * FROM PlayerActiveSpell WHERE PlayerId = ? ORDER BY PlayerActiveSpellId",
            playerId
        );
    }

    public PlayerActiveSpell GetByPlayerAndSpell(int playerId, int spellId)
    {
        List<PlayerActiveSpell> results = database.Query<PlayerActiveSpell>(
            "SELECT * FROM PlayerActiveSpell WHERE PlayerId = ? AND SpellId = ?",
            playerId,
            spellId
        );

        return results.Count > 0 ? results[0] : null;
    }

    public bool Update(PlayerActiveSpell playerActiveSpell)
    {
        return database.Update(playerActiveSpell) > 0;
    }

    public bool Delete(int playerActiveSpellId)
    {
        return database.Delete<PlayerActiveSpell>(playerActiveSpellId) > 0;
    }

    public int DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerActiveSpell WHERE PlayerId = ?",
            playerId
        );
    }

    public bool Exists(int playerId, int spellId)
    {
        return GetByPlayerAndSpell(playerId, spellId) != null;
    }

    public int CountByPlayerId(int playerId)
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM PlayerActiveSpell WHERE PlayerId = ?",
            playerId
        );
    }
}
