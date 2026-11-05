using SQLite;

[Table("Player")]
public class Player
{
    [PrimaryKey, AutoIncrement]
    public int PlayerId { get; set; }

    [NotNull]
    public string PlayerName { get; set; }

    [NotNull]
    public string CreatedAt { get; set; }
}