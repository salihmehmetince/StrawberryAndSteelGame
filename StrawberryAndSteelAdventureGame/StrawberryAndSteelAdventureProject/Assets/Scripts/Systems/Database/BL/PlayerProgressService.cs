
using System;

public class PlayerProgressService
{
    private const int TotalLevelCount = 10;

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
        if (playerId <= 0 || level < 1 || level > TotalLevelCount)
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
        if (playerId <= 0 || level < 1 || level > TotalLevelCount)
            return false;

        PlayerProgress progress = progressRepository.GetByPlayerId(playerId);

        if (progress == null)
            return false;

        // Daha önce açýlmýþ bir seviyeyi tekrar açmaya gerek yok.
        if (level <= progress.LastUnlockedLevel)
            return true;

        // Arada seviye atlayarak kilit açýlmasýný engelle.
        if (level != progress.LastUnlockedLevel + 1)
            return false;

        progress.LastUnlockedLevel = level;

        return progressRepository.Update(progress);
    }


    public bool CompleteLevel(int playerId, int completedLevel)
    {
        if (playerId <= 0 ||
            completedLevel < 1 ||
            completedLevel > TotalLevelCount)
            return false;

        PlayerProgress progress = progressRepository.GetByPlayerId(playerId);

        if (progress == null)
            return false;

        // Kilidi açýlmamýþ bir seviye tamamlanamaz.
        if (completedLevel > progress.LastUnlockedLevel)
            return false;

        // Daha önce tamamlanmýþ bir seviyenin tekrar bitirilmesi.
        // Mevcut ilerleme deðiþtirilmez.
        if (completedLevel < progress.CurrentLevel)
            return true;

        // Oyuncunun mevcut oynadýðý seviye olmalý.
        if (completedLevel != progress.CurrentLevel)
            return false;

        // Son seviyeden sonra yeni seviye oluþturma.
        if (completedLevel < TotalLevelCount)
        {
            int nextLevel = completedLevel + 1;

            if (nextLevel > progress.LastUnlockedLevel)
                progress.LastUnlockedLevel = nextLevel;

            progress.CurrentLevel = nextLevel;
        }

        return progressRepository.Update(progress);
    }

    public bool DeleteProgress(int playerId)
    {
        if (playerId <= 0)
            return false;

        return progressRepository.DeleteByPlayerId(playerId);
    }
}
