
using SQLite;
using System;

public class DatabaseSchemaVersionRepository
{
    private readonly SQLiteConnection database;
    private const int SchemaVersionId = 1;

    public DatabaseSchemaVersionRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int GetCurrentVersion()
    {
        DatabaseSchemaVersion schemaVersion =
            database.Find<DatabaseSchemaVersion>(SchemaVersionId);

        return schemaVersion != null ? schemaVersion.Version : 0;
    }

    public bool UpdateVersion(int version)
    {
        if (version < 1)
            return false;

        DatabaseSchemaVersion schemaVersion =
            database.Find<DatabaseSchemaVersion>(SchemaVersionId);

        if (schemaVersion == null)
        {
            schemaVersion = new DatabaseSchemaVersion
            {
                DatabaseSchemaVersionId = SchemaVersionId,
                Version = version,
                UpdatedAt = DateTime.UtcNow.ToString("o")
            };

            return database.Insert(schemaVersion) > 0;
        }

        schemaVersion.Version = version;
        schemaVersion.UpdatedAt = DateTime.UtcNow.ToString("o");

        return database.Update(schemaVersion) > 0;
    }

    public bool Exists()
    {
        return database.Find<DatabaseSchemaVersion>(
            SchemaVersionId
        ) != null;
    }
}
