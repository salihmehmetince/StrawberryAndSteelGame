
using System.Collections.Generic;
using SQLite;

public class PlayerHasSpellRepository
{
    private readonly SQLiteConnection database;

    public PlayerHasSpellRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerHasSpell playerHasSpell)
    {
        database.Insert(playerHasSpell);
        return playerHasSpell.PlayerHasSpellId;
    }

    public PlayerHasSpell GetById(int playerHasSpellId)
    {
        return database.Find<PlayerHasSpell>(playerHasSpellId);
    }

    public PlayerHasSpell GetByPlayerAndSpell(int playerId, int spellId)
    {
        List<PlayerHasSpell> results = database.Query<PlayerHasSpell>(
            "SELECT * FROM PlayerHasSpell WHERE PlayerId = ? AND SpellId = ?",
            playerId,
            spellId
        );

        return results.Count > 0 ? results[0] : null;
    }

    public List<PlayerHasSpell> GetByPlayerId(int playerId)
    {
        return database.Query<PlayerHasSpell>(
            "SELECT * FROM PlayerHasSpell WHERE PlayerId = ? ORDER BY PlayerHasSpellId",
            playerId
        );
    }

    public bool Update(PlayerHasSpell playerHasSpell)
    {
        return database.Update(playerHasSpell) > 0;
    }

    public bool Delete(int playerHasSpellId)
    {
        return database.Delete<PlayerHasSpell>(playerHasSpellId) > 0;
    }

    public int DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerHasSpell WHERE PlayerId = ?",
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
            "SELECT COUNT(*) FROM PlayerHasSpell WHERE PlayerId = ?",
            playerId
        );
    }
}
