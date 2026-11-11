using SQLite;

[Table("ItemStatModifier")]
public class ItemStatModifier
{
    [PrimaryKey, AutoIncrement]
    public int ItemStatModifierId { get; set; }

    [NotNull]
    public int ItemId { get; set; }

    [NotNull]
    public string StatType { get; set; }

    [NotNull]
    public float ModifierValue { get; set; }
}