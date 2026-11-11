using SQLite;

[Table("Item")]
public class Item
{
    [PrimaryKey, AutoIncrement]
    public int ItemId { get; set; }

    [NotNull, Unique]
    public string ItemName { get; set; }

    [NotNull]
    public int ItemTypeId { get; set; }

    public int? EquipmentSlotId { get; set; }

    public string ItemDescription { get; set; }

    [NotNull]
    public int IsActive { get; set; }
}