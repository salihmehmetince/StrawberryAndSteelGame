using SQLite;

[Table("ItemTypes")]
public class ItemType
{
    [PrimaryKey]
    public int ItemTypeId { get; set; }

    [NotNull, Unique]
    public string ItemTypeName { get; set; }
}