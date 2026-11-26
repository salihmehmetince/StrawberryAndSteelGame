
using SQLite;

[Table("PlayerMarketOffer")]
public class PlayerMarketOffer
{
    [PrimaryKey, AutoIncrement]
    public int PlayerMarketOfferId { get; set; }

    [NotNull]
    [Indexed("UX_PlayerMarketOffer_Slot", 1, Unique = true)]
    public int PlayerMarketId { get; set; }

    [NotNull]
    [Indexed("UX_PlayerMarketOffer_Slot", 2, Unique = true)]
    public int SlotNumber { get; set; }

    [NotNull]
    public int ItemId { get; set; }

    [NotNull]
    public int CoinPrice { get; set; }

    [NotNull]
    public int DiamondPrice { get; set; }

    [NotNull]
    public int IsPurchased { get; set; } = 0;

    public string PurchasedAtUtc { get; set; }
}
