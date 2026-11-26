
using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

public class PlayerMarketOfferRepository
{
    private readonly SQLiteConnection database;

    public PlayerMarketOfferRepository(DatabaseContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        database = context.Connection;
    }

    public int Add(PlayerMarketOffer offer)
    {
        if (offer == null)
            return 0;

        database.Insert(offer);
        return offer.PlayerMarketOfferId;
    }

    public PlayerMarketOffer GetById(int offerId)
    {
        if (offerId <= 0)
            return null;

        return database.Find<PlayerMarketOffer>(offerId);
    }

    public PlayerMarketOffer GetByMarketAndSlot(
        int playerMarketId,
        int slotNumber)
    {
        if (playerMarketId <= 0 || slotNumber < 1 || slotNumber > 6)
            return null;

        return database.Table<PlayerMarketOffer>()
            .FirstOrDefault(x =>
                x.PlayerMarketId == playerMarketId &&
                x.SlotNumber == slotNumber);
    }

    public List<PlayerMarketOffer> GetByMarketId(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return new List<PlayerMarketOffer>();

        return database.Table<PlayerMarketOffer>()
            .Where(x => x.PlayerMarketId == playerMarketId)
            .OrderBy(x => x.SlotNumber)
            .ToList();
    }

    public bool Update(PlayerMarketOffer offer)
    {
        if (offer == null || offer.PlayerMarketOfferId <= 0)
            return false;

        return database.Update(offer) > 0;
    }

    public bool Delete(int offerId)
    {
        if (offerId <= 0)
            return false;

        return database.Delete<PlayerMarketOffer>(offerId) > 0;
    }

    public int DeleteByMarketId(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return 0;

        List<PlayerMarketOffer> offers = GetByMarketId(playerMarketId);

        int deletedCount = 0;

        foreach (PlayerMarketOffer offer in offers)
        {
            if (database.Delete<PlayerMarketOffer>(
                offer.PlayerMarketOfferId) > 0)
            {
                deletedCount++;
            }
        }

        return deletedCount;
    }

    public bool ExistsInSlot(int playerMarketId, int slotNumber)
    {
        if (playerMarketId <= 0 || slotNumber < 1 || slotNumber > 6)
            return false;

        return database.Table<PlayerMarketOffer>()
            .Any(x =>
                x.PlayerMarketId == playerMarketId &&
                x.SlotNumber == slotNumber);
    }

    public int CountByMarketId(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return 0;

        return database.Table<PlayerMarketOffer>()
            .Count(x => x.PlayerMarketId == playerMarketId);
    }
}
