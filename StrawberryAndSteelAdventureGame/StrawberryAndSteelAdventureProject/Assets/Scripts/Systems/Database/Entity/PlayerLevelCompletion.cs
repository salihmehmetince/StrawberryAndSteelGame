using SQLite;

[Table("PlayerLevelCompletion")]
public class PlayerLevelCompletion
{
    [PrimaryKey, AutoIncrement]
    public int PlayerLevelCompletionId { get; set; }

    [NotNull]
    public int PlayerId { get; set; }

    [NotNull]
    public int LevelNumber { get; set; }

    [NotNull]
    public string CompletedAt { get; set; }
}