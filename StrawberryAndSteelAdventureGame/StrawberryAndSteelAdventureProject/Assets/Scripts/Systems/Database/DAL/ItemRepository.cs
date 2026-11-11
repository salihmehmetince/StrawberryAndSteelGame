
using System.Collections.Generic;
using SQLite;

public class ItemRepository
{
    private readonly SQLiteConnection database;

    public ItemRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(Item item)
    {
        database.Insert(item);
        return item.ItemId;
    }

    public Item GetById(int itemId)
    {
        return database.Find<Item>(itemId);
    }

    public List<Item> GetByTypeId(int itemTypeId)
    {
        return database.Query<Item>(
            "SELECT * FROM Item WHERE ItemTypeId = ? ORDER BY ItemId",
            itemTypeId
        );
    }

    public List<Item> GetByEquipmentSlotId(int equipmentSlotId)
    {
        return database.Query<Item>(
            "SELECT * FROM Item WHERE EquipmentSlotId = ? ORDER BY ItemId",
            equipmentSlotId
        );
    }

    public List<Item> GetAll()
    {
        return database.Query<Item>(
            "SELECT * FROM Item ORDER BY ItemId"
        );
    }

    public bool Update(Item item)
    {
        return database.Update(item) > 0;
    }

    public bool Delete(int itemId)
    {
        return database.Delete<Item>(itemId) > 0;
    }

    public bool Exists(int itemId)
    {
        return GetById(itemId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Item"
        );
    }
}
