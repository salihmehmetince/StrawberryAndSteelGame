
using SQLite;

[Table("MarketSettings")]
public class MarketSettings
{
    [PrimaryKey]
    public int MarketSettingsId { get; set; }

    [NotNull]
    public int BaseCoinPrice { get; set; } = 10;

    [NotNull]
    public int BaseDiamondPrice { get; set; } = 100;

    [NotNull]
    public int RefreshIntervalMinutes { get; set; } = 30;
}
