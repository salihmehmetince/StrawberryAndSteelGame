
using System.Collections.Generic;
using SQLite;

public class ShopOfferRepository
{
    private readonly SQLiteConnection database;

    public ShopOfferRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(ShopOffer shopOffer)
    {
        database.Insert(shopOffer);
        return shopOffer.ShopOfferId;
    }

    public ShopOffer GetById(int shopOfferId)
    {
        return database.Find<ShopOffer>(shopOfferId);
    }

    public List<ShopOffer> GetAll()
    {
        return database.Query<ShopOffer>(
            "SELECT * FROM ShopOffer ORDER BY ShopOfferId"
        );
    }

    public List<ShopOffer> GetActiveOffers()
    {
        return database.Query<ShopOffer>(
            "SELECT * FROM ShopOffer WHERE IsActive = 1 ORDER BY ShopOfferId"
        );
    }

    public List<ShopOffer> GetOffersByItemId(int itemId)
    {
        return database.Query<ShopOffer>(
            "SELECT * FROM ShopOffer WHERE ItemId = ? ORDER BY ShopOfferId",
            itemId
        );
    }

    public List<ShopOffer> GetOffersBySpellId(int spellId)
    {
        return database.Query<ShopOffer>(
            "SELECT * FROM ShopOffer WHERE SpellId = ? ORDER BY ShopOfferId",
            spellId
        );
    }

    public bool Update(ShopOffer shopOffer)
    {
        return database.Update(shopOffer) > 0;
    }

    public bool Delete(int shopOfferId)
    {
        return database.Delete<ShopOffer>(shopOfferId) > 0;
    }

    public bool Exists(int shopOfferId)
    {
        return GetById(shopOfferId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM ShopOffer"
        );
    }

    public int CountActiveOffers()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM ShopOffer WHERE IsActive = 1"
        );
    }
}
