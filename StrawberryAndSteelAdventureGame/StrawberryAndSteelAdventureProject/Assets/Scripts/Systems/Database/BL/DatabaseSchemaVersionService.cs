public class DatabaseSchemaVersionService
{
    private readonly DatabaseSchemaVersionRepository schemaVersionRepository;

    public DatabaseSchemaVersionService(
        DatabaseSchemaVersionRepository schemaVersionRepository)
    {
        this.schemaVersionRepository = schemaVersionRepository;
    }

    public int GetCurrentVersion()
    {
        return schemaVersionRepository.GetCurrentVersion();
    }

    public bool UpdateVersion(int version)
    {
        if (version < 1)
            return false;

        return schemaVersionRepository.UpdateVersion(version);
    }

    public bool VersionExists()
    {
        return schemaVersionRepository.Exists();
    }

    public bool IsVersionCurrent(int expectedVersion)
    {
        if (expectedVersion < 1)
            return false;

        return schemaVersionRepository.GetCurrentVersion() == expectedVersion;
    }

    public bool IsVersionOutdated(int expectedVersion)
    {
        if (expectedVersion < 1)
            return false;

        return schemaVersionRepository.GetCurrentVersion() < expectedVersion;
    }
}