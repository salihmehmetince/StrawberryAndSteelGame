using SQLite;

[Table("PlayerProgress")]
public class PlayerProgress
{
    [PrimaryKey, AutoIncrement]
    public int PlayerProgressId { get; set; }

    [NotNull, Unique]
    public int PlayerId { get; set; }

    [NotNull]
    public int LastUnlockedLevel { get; set; }

    [NotNull]
    public int CurrentLevel { get; set; }
}