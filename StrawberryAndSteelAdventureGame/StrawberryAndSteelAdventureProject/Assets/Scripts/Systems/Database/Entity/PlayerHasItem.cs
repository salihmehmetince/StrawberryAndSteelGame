using SQLite;

[Table("PlayerHasItem")]
public class PlayerHasItem
{
    [PrimaryKey, AutoIncrement]
    public int PlayerHasItemId { get; set; }

    [NotNull]
    public int PlayerId { get; set; }

    [NotNull]
    public int ItemId { get; set; }

    [NotNull]
    public int Quantity { get; set; }
}