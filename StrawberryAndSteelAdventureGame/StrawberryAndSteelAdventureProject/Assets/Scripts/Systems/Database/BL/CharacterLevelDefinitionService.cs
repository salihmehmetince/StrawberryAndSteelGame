using System.Collections.Generic;

public class CharacterLevelDefinitionService
{
    private readonly CharacterLevelDefinitionRepository definitionRepository;

    public CharacterLevelDefinitionService(
        CharacterLevelDefinitionRepository definitionRepository)
    {
        this.definitionRepository = definitionRepository;
    }

    public CharacterLevelDefinition GetDefinitionById(int definitionId)
    {
        if (definitionId <= 0)
            return null;

        return definitionRepository.GetById(definitionId);
    }

    public CharacterLevelDefinition GetDefinitionByLevel(int level)
    {
        if (level <= 0)
            return null;

        return definitionRepository.GetByLevel(level);
    }

    public CharacterLevelDefinition GetDefinitionByTotalXP(long totalXP)
    {
        if (totalXP < 0)
            return null;

        return definitionRepository.GetByRequiredTotalXP(totalXP);
    }

    public List<CharacterLevelDefinition> GetAllDefinitions()
    {
        return definitionRepository.GetAll();
    }

    public int GetCharacterLevel(long totalXP)
    {
        if (totalXP < 0)
            return 0;

        CharacterLevelDefinition definition =
            definitionRepository.GetByRequiredTotalXP(totalXP);

        return definition != null ? definition.Level : 0;
    }

    public long GetXPRequiredForNextLevel(long totalXP)
    {
        if (totalXP < 0)
            return -1;

        CharacterLevelDefinition currentDefinition =
            definitionRepository.GetByRequiredTotalXP(totalXP);

        if (currentDefinition == null)
            return -1;

        CharacterLevelDefinition nextDefinition =
            definitionRepository.GetByLevel(currentDefinition.Level + 1);

        if (nextDefinition == null)
            return -1;

        return nextDefinition.RequiredTotalXP - totalXP;
    }

    public int CreateDefinition(CharacterLevelDefinition definition)
    {
        if (definition == null ||
            definition.Level <= 0 ||
            definition.RequiredTotalXP < 0)
            return 0;

        if (definitionRepository.Exists(definition.Level))
            return 0;

        return definitionRepository.Add(definition);
    }

    public bool UpdateDefinition(CharacterLevelDefinition definition)
    {
        if (definition == null ||
            definition.CharacterLevelDefinitionId <= 0 ||
            definition.Level <= 0 ||
            definition.RequiredTotalXP < 0)
            return false;

        return definitionRepository.Update(definition);
    }

    public bool DeleteDefinition(int definitionId)
    {
        if (definitionId <= 0)
            return false;

        return definitionRepository.Delete(definitionId);
    }

    public bool DefinitionExists(int level)
    {
        if (level <= 0)
            return false;

        return definitionRepository.Exists(level);
    }

    public int GetDefinitionCount()
    {
        return definitionRepository.Count();
    }
}