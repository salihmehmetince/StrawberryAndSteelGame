using SQLite;

[Table("PlayerSettings")]
public class PlayerSettings
{
    [PrimaryKey]
    public int PlayerId { get; set; }

    [NotNull]
    public float AudioLevel { get; set; }

    [NotNull]
    public float MusicLevel { get; set; }

    [NotNull]
    public float Sensitivity { get; set; }
}