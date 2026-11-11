
using System.Collections.Generic;
using SQLite;

public class SpellRepository
{
    private readonly SQLiteConnection database;

    public SpellRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(Spell spell)
    {
        database.Insert(spell);
        return spell.SpellId;
    }

    public Spell GetById(int spellId)
    {
        return database.Find<Spell>(spellId);
    }

    public List<Spell> GetAll()
    {
        return database.Query<Spell>(
            "SELECT * FROM Spell ORDER BY SpellId"
        );
    }

    public bool Update(Spell spell)
    {
        return database.Update(spell) > 0;
    }

    public bool Delete(int spellId)
    {
        return database.Delete<Spell>(spellId) > 0;
    }

    public bool Exists(int spellId)
    {
        return GetById(spellId) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Spell"
        );
    }
}
