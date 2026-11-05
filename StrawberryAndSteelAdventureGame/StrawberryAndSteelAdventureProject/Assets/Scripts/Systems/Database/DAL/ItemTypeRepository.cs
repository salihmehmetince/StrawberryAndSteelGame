using System.Collections.Generic;
using SQLite;

public class ItemTypeRepository
{
    private readonly SQLiteConnection database;

    public ItemTypeRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(ItemType itemType)
    {
        database.Insert(itemType);
        return itemType.ItemTypeId;
    }

    public ItemType GetById(int itemTypeId)
    {
        return database.Find<ItemType>(itemTypeId);
    }

    public ItemType GetByName(string itemTypeName)
    {
        List<ItemType> results = database.Query<ItemType>(
            "SELECT * FROM ItemTypes WHERE ItemTypeName = ?",
            itemTypeName
        );

        return results.Count > 0 ? results[0] : null;
    }

    public List<ItemType> GetAll()
    {
        return database.Query<ItemType>(
            "SELECT * FROM ItemTypes ORDER BY ItemTypeId"
        );
    }

    public bool Update(ItemType itemType)
    {
        return database.Update(itemType) > 0;
    }

    public bool Delete(int itemTypeId)
    {
        return database.Delete<ItemType>(itemTypeId) > 0;
    }

    public bool Exists(int itemTypeId)
    {
        return GetById(itemTypeId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM ItemTypes"
        );
    }
}