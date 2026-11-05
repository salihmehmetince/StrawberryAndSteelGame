
using SQLite;

[Table("GameSettings")]
public class GameSettings
{
    [PrimaryKey]
    public int GameSettingsId { get; set; }

    [NotNull]
    public float AudioLevel { get; set; }

    [NotNull]
    public float MusicLevel { get; set; }

    [NotNull]
    public float Sensitivity { get; set; }
}
