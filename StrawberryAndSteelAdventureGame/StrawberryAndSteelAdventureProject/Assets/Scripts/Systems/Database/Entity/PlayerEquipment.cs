using SQLite;

[Table("PlayerEquipment")]
public class PlayerEquipment
{
    [PrimaryKey, AutoIncrement]
    public int PlayerEquipmentId { get; set; }

    [NotNull]
    public int PlayerId { get; set; }

    [NotNull]
    public int PlayerHasItemId { get; set; }
}