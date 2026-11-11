
using System.Collections.Generic;
using SQLite;

public class SpellEffectRepository
{
    private readonly SQLiteConnection database;

    public SpellEffectRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(SpellEffect spellEffect)
    {
        database.Insert(spellEffect);
        return spellEffect.SpellEffectId;
    }

    public SpellEffect GetById(int spellEffectId)
    {
        return database.Find<SpellEffect>(spellEffectId);
    }

    public List<SpellEffect> GetBySpellId(int spellId)
    {
        return database.Query<SpellEffect>(
            "SELECT * FROM SpellEffect WHERE SpellId = ? ORDER BY SpellEffectId",
            spellId
        );
    }

    public List<SpellEffect> GetAll()
    {
        return database.Query<SpellEffect>(
            "SELECT * FROM SpellEffect ORDER BY SpellEffectId"
        );
    }

    public bool Update(SpellEffect spellEffect)
    {
        return database.Update(spellEffect) > 0;
    }

    public bool Delete(int spellEffectId)
    {
        return database.Delete<SpellEffect>(spellEffectId) > 0;
    }

    public int DeleteBySpellId(int spellId)
    {
        return database.Execute(
            "DELETE FROM SpellEffect WHERE SpellId = ?",
            spellId
        );
    }

    public bool Exists(int spellEffectId)
    {
        return GetById(spellEffectId) != null;
    }

    public int CountBySpellId(int spellId)
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SpellEffect WHERE SpellId = ?",
            spellId
        );
    }
}
