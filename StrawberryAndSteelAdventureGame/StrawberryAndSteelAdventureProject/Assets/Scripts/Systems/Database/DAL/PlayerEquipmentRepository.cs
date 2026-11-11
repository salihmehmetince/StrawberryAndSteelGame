
using SQLite;
using System.Collections.Generic;
using System.Linq;

public class PlayerEquipmentRepository
{
    private readonly SQLiteConnection database;

    public PlayerEquipmentRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(PlayerEquipment playerEquipment)
    {
        database.Insert(playerEquipment);
        return playerEquipment.PlayerEquipmentId;
    }

    public PlayerEquipment GetById(int playerEquipmentId)
    {
        return database.Find<PlayerEquipment>(playerEquipmentId);
    }

    public List<PlayerEquipment> GetByPlayerId(int playerId)
    {
        return database.Query<PlayerEquipment>(
            "SELECT * FROM PlayerEquipment WHERE PlayerId = ? ORDER BY EquipmentSlotId",
            playerId
        );
    }

    public PlayerEquipment GetByPlayerAndSlot(int playerId, int equipmentSlotId)
    {
        return database.Query<PlayerEquipment>(
            "SELECT * FROM PlayerEquipment WHERE PlayerId = ? AND EquipmentSlotId = ?",
            playerId,
            equipmentSlotId
        ).FirstOrDefault();
    }

    public bool Update(PlayerEquipment playerEquipment)
    {
        return database.Update(playerEquipment) > 0;
    }

    public bool Delete(int playerEquipmentId)
    {
        return database.Delete<PlayerEquipment>(playerEquipmentId) > 0;
    }

    public int DeleteByPlayerId(int playerId)
    {
        return database.Execute(
            "DELETE FROM PlayerEquipment WHERE PlayerId = ?",
            playerId
        );
    }

    public bool Exists(int playerId, int equipmentSlotId)
    {
        return GetByPlayerAndSlot(playerId, equipmentSlotId) != null;
    }

    public int CountByPlayerId(int playerId)
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM PlayerEquipment WHERE PlayerId = ?",
            playerId
        );
    }
}
