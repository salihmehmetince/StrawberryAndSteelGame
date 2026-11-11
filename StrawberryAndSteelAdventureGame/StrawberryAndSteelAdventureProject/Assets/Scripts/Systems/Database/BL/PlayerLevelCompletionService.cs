using System.Collections.Generic;

public class PlayerLevelCompletionService
{
    private readonly PlayerLevelCompletionRepository completionRepository;

    public PlayerLevelCompletionService(
        PlayerLevelCompletionRepository completionRepository)
    {
        this.completionRepository = completionRepository;
    }

    public PlayerLevelCompletion GetCompletionById(int completionId)
    {
        if (completionId <= 0)
            return null;

        return completionRepository.GetById(completionId);
    }

    public PlayerLevelCompletion GetCompletion(
        int playerId,
        int levelNumber)
    {
        if (playerId <= 0 || levelNumber <= 0)
            return null;

        return completionRepository.GetByPlayerAndLevel(
            playerId,
            levelNumber
        );
    }

    public List<PlayerLevelCompletion> GetPlayerCompletions(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerLevelCompletion>();

        return completionRepository.GetByPlayerId(playerId);
    }

    public int CompleteLevel(int playerId, int levelNumber)
    {
        if (playerId <= 0 || levelNumber <= 0)
            return 0;

        if (completionRepository.Exists(playerId, levelNumber))
            return 0;

        PlayerLevelCompletion completion = new PlayerLevelCompletion
        {
            PlayerId = playerId,
            LevelNumber = levelNumber
        };

        return completionRepository.Add(completion);
    }

    public bool UpdateCompletion(PlayerLevelCompletion completion)
    {
        if (completion == null ||
            completion.PlayerLevelCompletionId <= 0 ||
            completion.PlayerId <= 0 ||
            completion.LevelNumber <= 0)
            return false;

        PlayerLevelCompletion existingCompletion =
            completionRepository.GetByPlayerAndLevel(
                completion.PlayerId,
                completion.LevelNumber
            );

        if (existingCompletion != null &&
            existingCompletion.PlayerLevelCompletionId !=
            completion.PlayerLevelCompletionId)
            return false;

        return completionRepository.Update(completion);
    }

    public bool DeleteCompletion(int completionId)
    {
        if (completionId <= 0)
            return false;

        return completionRepository.Delete(completionId);
    }

    public int DeletePlayerCompletions(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return completionRepository.DeleteByPlayerId(playerId);
    }

    public bool IsLevelCompleted(int playerId, int levelNumber)
    {
        if (playerId <= 0 || levelNumber <= 0)
            return false;

        return completionRepository.Exists(playerId, levelNumber);
    }

    public int GetCompletedLevelCount(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return completionRepository.CountByPlayerId(playerId);
    }
}