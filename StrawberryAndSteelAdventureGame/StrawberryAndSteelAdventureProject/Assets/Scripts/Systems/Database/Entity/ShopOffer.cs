using SQLite;

[Table("ShopOffer")]
public class ShopOffer
{
    [PrimaryKey, AutoIncrement]
    public int ShopOfferId { get; set; }

    public int? ItemId { get; set; }

    public int? SpellId { get; set; }

    [NotNull]
    public int CoinPrice { get; set; }

    [NotNull]
    public int DiamondPrice { get; set; }

    [NotNull]
    public int IsActive { get; set; }
}