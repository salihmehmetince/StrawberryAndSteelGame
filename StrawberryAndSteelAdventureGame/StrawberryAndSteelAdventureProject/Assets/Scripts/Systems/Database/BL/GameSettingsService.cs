
using System;

public class GameSettingsService
{
    private readonly GameSettingsRepository gameSettingsRepository;

    public GameSettingsService(GameSettingsRepository gameSettingsRepository)
    {
        this.gameSettingsRepository = gameSettingsRepository;
    }

    public GameSettings GetSettings()
    {
        return gameSettingsRepository.GetSettings();
    }

    public bool EnsureDefaultSettings()
    {
        if (gameSettingsRepository.Exists())
            return true;

        GameSettings settings = new GameSettings
        {
            GameSettingsId = 1,
            AudioLevel = 1f,
            MusicLevel = 1f,
            Sensitivity = 1f
        };

        return gameSettingsRepository.Add(settings) > 0;
    }

    public bool UpdateSettings(
        float audioLevel,
        float musicLevel,
        float sensitivity)
    {
        if (!IsValidLevel(audioLevel) ||
            !IsValidLevel(musicLevel) ||
            !IsValidLevel(sensitivity))
        {
            return false;
        }

        GameSettings settings = gameSettingsRepository.GetSettings();

        if (settings == null)
            return false;

        settings.AudioLevel = audioLevel;
        settings.MusicLevel = musicLevel;
        settings.Sensitivity = sensitivity;

        return gameSettingsRepository.Update(settings);
    }

    public bool DeleteSettings()
    {
        return gameSettingsRepository.Delete();
    }

    private bool IsValidLevel(float value)
    {
        return value >= 0f && value <= 1f;
    }
}
