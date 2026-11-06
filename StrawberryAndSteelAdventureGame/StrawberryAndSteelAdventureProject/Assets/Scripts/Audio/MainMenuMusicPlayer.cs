
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuMusicPlayer : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private AudioClip[] musicClips;

    private AudioSource audioSource;
    private int[] musicOrder;
    private int currentMusicIndex;
    private int lastMusicIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void Start()
    {
        LoadMusicVolume();

        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("Herhangi bir müzik eklenmemiþ.");
            return;
        }

        CreateMusicOrder();
        PlayNextMusic();
    }

    private void Update()
    {
        if (musicClips == null || musicClips.Length == 0)
            return;

        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            PlayNextMusic();
        }
    }

    private void LoadMusicVolume()
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

        SetMusicVolume(settings.MusicLevel);
    }

    public void SetMusicVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    private void CreateMusicOrder()
    {
        musicOrder = new int[musicClips.Length];

        for (int i = 0; i < musicOrder.Length; i++)
        {
            musicOrder[i] = i;
        }

        // Fisher-Yates karýþtýrma algoritmasý.
        for (int i = musicOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = musicOrder[i];
            musicOrder[i] = musicOrder[randomIndex];
            musicOrder[randomIndex] = temp;
        }

        currentMusicIndex = 0;

        // Yeni turun ilk müziði, önceki turun son müziði olmasýn.
        if (musicOrder.Length > 1 &&
            musicOrder[0] == lastMusicIndex)
        {
            int temp = musicOrder[0];
            musicOrder[0] = musicOrder[1];
            musicOrder[1] = temp;
        }
    }

    private void PlayNextMusic()
    {
        if (currentMusicIndex >= musicOrder.Length)
        {
            CreateMusicOrder();
        }

        int selectedIndex = musicOrder[currentMusicIndex];
        currentMusicIndex++;

        AudioClip selectedMusic = musicClips[selectedIndex];

        if (selectedMusic == null)
        {
            PlayNextMusic();
            return;
        }

        lastMusicIndex = selectedIndex;

        audioSource.clip = selectedMusic;
        audioSource.Play();
    }
}
