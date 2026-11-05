
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

        connection.Execute("PRAGMA foreign_keys = ON;");

        UnityEngine.Debug.Log(
            "SQLite connection established: " + databasePath
        );
    }

    public void RunInTransaction(Action action)
    {
        connection.RunInTransaction(action);
    }

    public void Dispose()
    {
        connection?.Close();
        connection?.Dispose();
    }
}
