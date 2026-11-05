using System;

public class PlayerSettingsService
{
    private readonly PlayerSettingsRepository settingsRepository;

    public PlayerSettingsService(PlayerSettingsRepository settingsRepository)
    {
        this.settingsRepository = settingsRepository;
    }

    public PlayerSettings GetSettings(int playerId)
    {
        if (playerId <= 0)
            return null;

        return settingsRepository.GetByPlayerId(playerId);
    }

    public int CreateSettings(int playerId)
    {
        if (playerId <= 0)
            throw new ArgumentException("Invalid player ID.");

        if (settingsRepository.Exists(playerId))
            throw new InvalidOperationException("Player settings already exist.");

        PlayerSettings settings = new PlayerSettings
        {
            PlayerId = playerId,
            AudioLevel = 1f,
            MusicLevel = 1f,
            Sensitivity = 1f
        };

        return settingsRepository.Add(settings);
    }

    public bool UpdateSettings(PlayerSettings settings)
    {
        if (settings == null || settings.PlayerId <= 0)
            return false;

        if (settings.AudioLevel < 0f || settings.AudioLevel > 1f)
            return false;

        if (settings.MusicLevel < 0f || settings.MusicLevel > 1f)
            return false;

        if (settings.Sensitivity < 0f || settings.Sensitivity > 1f)
            return false;

        return settingsRepository.Update(settings);
    }

    public bool DeleteSettings(int playerId)
    {
        if (playerId <= 0)
            return false;

        return settingsRepository.Delete(playerId);
    }
}