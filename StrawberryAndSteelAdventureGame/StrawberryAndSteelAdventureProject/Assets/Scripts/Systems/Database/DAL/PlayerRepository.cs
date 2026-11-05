
using System.Collections.Generic;
using System.Linq;
using SQLite;

public class PlayerRepository
{
    private readonly SQLiteConnection database;

    public PlayerRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(Player player)
    {
        database.Insert(player);
        return player.PlayerId;
    }

    public Player GetById(int playerId)
    {
        return database.Find<Player>(playerId);
    }

    public List<Player> GetAll()
    {
        return database.Query<Player>(
            "SELECT * FROM Player ORDER BY PlayerId LIMIT 3"
        );
    }

    public bool Update(Player player)
    {
        return database.Update(player) > 0;
    }

    public bool Delete(int playerId)
    {
        return database.Delete<Player>(playerId) > 0;
    }

    public bool Exists(int playerId)
    {
        return database.Find<Player>(playerId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Player"
        );
    }
}
