
using SQLite;

[Table("PlayerMarket")]
public class PlayerMarket
{
    [PrimaryKey, AutoIncrement]
    public int PlayerMarketId { get; set; }

    [NotNull, Unique]
    public int PlayerId { get; set; }

    [NotNull]
    public string CreatedAtUtc { get; set; }

    [NotNull]
    public string NextRefreshAtUtc { get; set; }
}
