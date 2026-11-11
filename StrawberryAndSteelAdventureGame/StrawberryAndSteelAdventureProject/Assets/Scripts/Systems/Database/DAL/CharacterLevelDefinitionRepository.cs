
using System.Collections.Generic;
using SQLite;

public class CharacterLevelDefinitionRepository
{
    private readonly SQLiteConnection database;

    public CharacterLevelDefinitionRepository(DatabaseContext context)
    {
        database = context.Connection;
    }

    public int Add(CharacterLevelDefinition definition)
    {
        database.Insert(definition);
        return definition.CharacterLevelDefinitionId;
    }

    public CharacterLevelDefinition GetById(int definitionId)
    {
        return database.Find<CharacterLevelDefinition>(definitionId);
    }

    public CharacterLevelDefinition GetByLevel(int level)
    {
        List<CharacterLevelDefinition> results =
            database.Query<CharacterLevelDefinition>(
                "SELECT * FROM CharacterLevelDefinition WHERE Level = ?",
                level
            );

        return results.Count > 0 ? results[0] : null;
    }

    public CharacterLevelDefinition GetByRequiredTotalXP(long totalXP)
    {
        List<CharacterLevelDefinition> results =
            database.Query<CharacterLevelDefinition>(
                @"SELECT * FROM CharacterLevelDefinition
                  WHERE RequiredTotalXP <= ?
                  ORDER BY RequiredTotalXP DESC
                  LIMIT 1",
                totalXP
            );

        return results.Count > 0 ? results[0] : null;
    }

    public List<CharacterLevelDefinition> GetAll()
    {
        return database.Query<CharacterLevelDefinition>(
            "SELECT * FROM CharacterLevelDefinition ORDER BY Level"
        );
    }

    public bool Update(CharacterLevelDefinition definition)
    {
        return database.Update(definition) > 0;
    }

    public bool Delete(int definitionId)
    {
        return database.Delete<CharacterLevelDefinition>(definitionId) > 0;
    }

    public bool Exists(int level)
    {
        return GetByLevel(level) != null;
    }

    public int Count()
    {
        return database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM CharacterLevelDefinition"
        );
    }
}
