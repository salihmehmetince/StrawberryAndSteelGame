
using System;

public class PlayerProgressService
{
    private readonly PlayerProgressRepository progressRepository;

    public PlayerProgressService(PlayerProgressRepository progressRepository)
    {
        this.progressRepository = progressRepository;
    }

    public PlayerProgress GetProgress(int playerId)
    {
        if (playerId <= 0)
            return null;

        return progressRepository.GetByPlayerId(playerId);
    }

    public int CreateProgress(int playerId)
    {
        if (playerId <= 0)
            throw new ArgumentException("Invalid player ID.");

        if (progressRepository.Exists(playerId))
            throw new InvalidOperationException("Player progress already exists.");

        PlayerProgress progress = new PlayerProgress
        {
            PlayerId = playerId,
            LastUnlockedLevel = 1,
            CurrentLevel = 1
        };

        return progressRepository.Add(progress);
    }

    public bool UpdateCurrentLevel(int playerId, int level)
    {
        if (playerId <= 0 || level <= 0)
            return false;

        PlayerProgress progress = progressRepository.GetByPlayerId(playerId);

        if (progress == null)
            return false;

        if (level > progress.LastUnlockedLevel)
            return false;

        progress.CurrentLevel = level;

        return progressRepository.Update(progress);
    }

    public bool UnlockLevel(int playerId, int level)
    {
        if (playerId <= 0 || level <= 0)
            return false;

        PlayerProgress progress = progressRepository.GetByPlayerId(playerId);

        if (progress == null)
            return false;

        if (level > progress.LastUnlockedLevel)
            progress.LastUnlockedLevel = level;

        return progressRepository.Update(progress);
    }

    public bool CompleteLevel(int playerId, int completedLevel)
    {
        if (playerId <= 0 || completedLevel <= 0)
            return false;

        PlayerProgress progress = progressRepository.GetByPlayerId(playerId);

        if (progress == null)
            return false;

        int nextLevel = completedLevel + 1;

        if (nextLevel > progress.LastUnlockedLevel)
            progress.LastUnlockedLevel = nextLevel;

        progress.CurrentLevel = nextLevel;

        return progressRepository.Update(progress);
    }

    public bool DeleteProgress(int playerId)
    {
        if (playerId <= 0)
            return false;

        return progressRepository.DeleteByPlayerId(playerId);
    }
}
