using SQLite;

[Table("DatabaseSchemaVersion")]
public class DatabaseSchemaVersion
{
    [PrimaryKey]
    public int DatabaseSchemaVersionId { get; set; }

    [NotNull]
    public int Version { get; set; }

    [NotNull]
    public string UpdatedAt { get; set; }
}