
using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    public static MainMenuAudioController Instance { get; private set; }

    [Header("Sound Effect Audio Source")]
    [SerializeField] private AudioSource SoundEffectAudioSource;

    [Header("Button Sounds")]
    [SerializeField] private AudioClip ButtonHoverClip;
    [SerializeField] private AudioClip ButtonClickClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadAudioVolume();
    }

    private void LoadAudioVolume()
    {
        if (GameBootstrapper.Instance == null)
        {
            Debug.LogError("GameBootstrapper instance could not be found.");
            return;
        }

        GameSettingsService gameSettingsService =
            GameBootstrapper.Instance.GameSettingsService;

        if (gameSettingsService == null)
        {
            Debug.LogError("GameSettingsService could not be found.");
            return;
        }

        GameSettings settings = gameSettingsService.GetSettings();

        if (settings == null)
        {
            Debug.LogWarning("Game settings not found. Creating default settings.");

            if (!gameSettingsService.EnsureDefaultSettings())
            {
                Debug.LogError("Default game settings could not be created.");
                return;
            }

            settings = gameSettingsService.GetSettings();
        }

        if (settings == null)
        {
            Debug.LogError("Game settings could not be loaded.");
            return;
        }

        SetAudioVolume(settings.AudioLevel);
    }

    public void SetAudioVolume(float volume)
    {
        if (SoundEffectAudioSource == null)
            return;

        SoundEffectAudioSource.volume = Mathf.Clamp01(volume);
    }

    public void PlayButtonHover()
    {
        if (SoundEffectAudioSource == null || ButtonHoverClip == null)
            return;

        SoundEffectAudioSource.PlayOneShot(ButtonHoverClip);
    }

    public void PlayButtonClick()
    {
        if (SoundEffectAudioSource == null || ButtonClickClip == null)
            return;

        SoundEffectAudioSource.PlayOneShot(ButtonClickClip);
    }
}
