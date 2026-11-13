using SQLite;

[Table("PlayerCharacterStats")]
public class PlayerCharacterStats
{
    [PrimaryKey, AutoIncrement]
    public int PlayerCharacterStatsId { get; set; }

    [NotNull, Unique]
    public int PlayerId { get; set; }

    [NotNull]
    public long TotalXP { get; set; }
}