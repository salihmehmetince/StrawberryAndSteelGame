using System.Collections.Generic;
using SQLite;

public class PlayerHasItemRepository
{
    private readonly SQLiteConnection database;

    public PlayerHasItemRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerHasItem playerHasItem)
    {
        database.Insert(playerHasItem);
        return playerHasItem.PlayerHasItemId;
    }

    public PlayerHasItem GetById(int playerHasItemId)
    {
        return database.Find<PlayerHasItem>(playerHasItemId);
    }

    public PlayerHasItem GetByPlayerAndItem(int playerId, int itemId)
    {
        List<PlayerHasItem> results = database.Query<PlayerHasItem>(
            "SELECT * FROM PlayerHasItem WHERE PlayerId = ? AND ItemId = ?",
            playerId,
            itemId
        );

        return results.Count > 0 ? results[0] : null;
    }

    public List<PlayerHasItem> GetByPlayerId(int playerId)
    {
        return database.Query<PlayerHasItem>(
            "SELECT * FROM PlayerHasItem WHERE PlayerId = ? ORDER BY PlayerHasItemId",
            playerId
        );
    }

    public bool Update(PlayerHasItem playerHasItem)
    {
        return database.Update(playerHasItem) > 0;
    }

    public bool Delete(int playerHasItemId)
    {
        return database.Delete<PlayerHasItem>(playerHasItemId) > 0;
    }

    public int DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerHasItem WHERE PlayerId = ?",
            playerId
        );
    }

    public bool Exists(int playerId, int itemId)
    {
        return GetByPlayerAndItem(playerId, itemId) != null;
    }

    public int CountByPlayerId(int playerId)
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM PlayerHasItem WHERE PlayerId = ?",
            playerId
        );
    }
}