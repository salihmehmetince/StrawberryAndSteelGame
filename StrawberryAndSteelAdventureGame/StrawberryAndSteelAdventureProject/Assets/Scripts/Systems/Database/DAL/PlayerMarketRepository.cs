
using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

public class PlayerMarketRepository
{
    private readonly SQLiteConnection database;

    public PlayerMarketRepository(DatabaseContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        database = context.Connection;
    }

    public int Add(PlayerMarket market)
    {
        if (market == null)
            return 0;

        database.Insert(market);
        return market.PlayerMarketId;
    }

    public PlayerMarket GetById(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return null;

        return database.Find<PlayerMarket>(playerMarketId);
    }

    public PlayerMarket GetByPlayerId(int playerId)
    {
        if (playerId <= 0)
            return null;

        return database.Table<PlayerMarket>()
            .FirstOrDefault(x => x.PlayerId == playerId);
    }

    public List<PlayerMarket> GetAll()
    {
        return database.Table<PlayerMarket>().ToList();
    }

    public bool Update(PlayerMarket market)
    {
        if (market == null || market.PlayerMarketId <= 0)
            return false;

        return database.Update(market) > 0;
    }

    public bool Delete(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return false;

        return database.Delete<PlayerMarket>(playerMarketId) > 0;
    }

    public bool Exists(int playerId)
    {
        if (playerId <= 0)
            return false;

        return database.Table<PlayerMarket>()
            .Any(x => x.PlayerId == playerId);
    }

    public int Count()
    {
        return database.Table<PlayerMarket>().Count();
    }
}
