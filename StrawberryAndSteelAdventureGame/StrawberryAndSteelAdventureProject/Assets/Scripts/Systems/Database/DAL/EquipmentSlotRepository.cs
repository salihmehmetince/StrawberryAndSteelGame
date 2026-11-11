
using System.Collections.Generic;
using SQLite;

public class EquipmentSlotRepository
{
    private readonly SQLiteConnection database;

    public EquipmentSlotRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(EquipmentSlot equipmentSlot)
    {
        database.Insert(equipmentSlot);
        return equipmentSlot.EquipmentSlotId;
    }

    public EquipmentSlot GetById(int equipmentSlotId)
    {
        return database.Find<EquipmentSlot>(equipmentSlotId);
    }

    public EquipmentSlot GetByName(string slotName)
    {
        List<EquipmentSlot> results = database.Query<EquipmentSlot>(
            "SELECT * FROM EquipmentSlot WHERE SlotName = ?",
            slotName
        );

        return results.Count > 0 ? results[0] : null;
    }

    public List<EquipmentSlot> GetAll()
    {
        return database.Query<EquipmentSlot>(
            "SELECT * FROM EquipmentSlot ORDER BY EquipmentSlotId"
        );
    }

    public bool Update(EquipmentSlot equipmentSlot)
    {
        return database.Update(equipmentSlot) > 0;
    }

    public bool Delete(int equipmentSlotId)
    {
        return database.Delete<EquipmentSlot>(equipmentSlotId) > 0;
    }

    public bool Exists(int equipmentSlotId)
    {
        return GetById(equipmentSlotId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM EquipmentSlot"
        );
    }
}
