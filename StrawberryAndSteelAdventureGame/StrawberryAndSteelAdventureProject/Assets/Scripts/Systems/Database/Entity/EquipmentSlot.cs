using SQLite;

[Table("EquipmentSlot")]
public class EquipmentSlot
{
    [PrimaryKey, AutoIncrement]
    public int EquipmentSlotId { get; set; }

    [NotNull, Unique]
    public string SlotName { get; set; }
}