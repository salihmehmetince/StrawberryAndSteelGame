
using System.Collections.Generic;
using SQLite;

public class ItemStatModifierRepository
{
    private readonly SQLiteConnection database;

    public ItemStatModifierRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(ItemStatModifier itemStatModifier)
    {
        database.Insert(itemStatModifier);
        return itemStatModifier.ItemStatModifierId;
    }

    public ItemStatModifier GetById(int itemStatModifierId)
    {
        return database.Find<ItemStatModifier>(itemStatModifierId);
    }

    public List<ItemStatModifier> GetByItemId(int itemId)
    {
        return database.Query<ItemStatModifier>(
            "SELECT * FROM ItemStatModifier WHERE ItemId = ? ORDER BY ItemStatModifierId",
            itemId
        );
    }

    public List<ItemStatModifier> GetAll()
    {
        return database.Query<ItemStatModifier>(
            "SELECT * FROM ItemStatModifier ORDER BY ItemStatModifierId"
        );
    }

    public bool Update(ItemStatModifier itemStatModifier)
    {
        return database.Update(itemStatModifier) > 0;
    }

    public bool Delete(int itemStatModifierId)
    {
        return database.Delete<ItemStatModifier>(itemStatModifierId) > 0;
    }

    public int DeleteByItemId(int itemId)
    {
        return database.Execute(
            "DELETE FROM ItemStatModifier WHERE ItemId = ?",
            itemId
        );
    }

    public bool Exists(int itemStatModifierId)
    {
        return GetById(itemStatModifierId) != null;
    }

    public int CountByItemId(int itemId)
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM ItemStatModifier WHERE ItemId = ?",
            itemId
        );
    }
}
