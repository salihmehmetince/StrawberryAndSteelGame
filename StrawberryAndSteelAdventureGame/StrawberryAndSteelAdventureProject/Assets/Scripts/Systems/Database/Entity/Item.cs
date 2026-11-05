using SQLite;

[Table("Item")]
public class Item
{
    [PrimaryKey]
    public int ItemId { get; set; }

    [NotNull]
    public string ItemName { get; set; }

    [NotNull]
    public int ItemTypeId { get; set; }

    public string ItemDescription { get; set; }

    [NotNull]
    public int ItemValue { get; set; }

    [NotNull]
    public int IsStackable { get; set; }

    [NotNull]
    public int MaxStack { get; set; }
}