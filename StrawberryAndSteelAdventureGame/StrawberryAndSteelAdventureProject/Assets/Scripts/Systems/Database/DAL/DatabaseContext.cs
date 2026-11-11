
using System;
using System.IO;
using SQLite;

public sealed class DatabaseContext : IDisposable
{
    private readonly SQLiteConnection connection;

    public SQLiteConnection Connection => connection;

    public DatabaseContext(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("Database name cannot be empty.");

        string databasePath = Path.Combine(
            UnityEngine.Application.persistentDataPath,
            databaseName
        );

        connection = new SQLiteConnection(databasePath);

        ConfigureDatabase();

        UnityEngine.Debug.Log(
            "SQLite connection established: " + databasePath
        );
    }

    private void ConfigureDatabase()
    {
        connection.Execute("PRAGMA foreign_keys = ON;");
        connection.Execute("PRAGMA journal_mode = WAL;");
        connection.Execute("PRAGMA synchronous = NORMAL;");
    }

    public void RunInTransaction(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        connection.RunInTransaction(action);
    }

    public void Dispose()
    {
        if (connection == null)
            return;

        connection.Close();
        connection.Dispose();
    }
}
