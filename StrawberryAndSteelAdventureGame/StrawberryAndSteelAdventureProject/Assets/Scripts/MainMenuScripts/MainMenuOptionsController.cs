
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOptionsController : MonoBehaviour
{
    [Header("Game Settings")]
    private GameSettingsService gameSettingsService;

    [Header("Options Panel")]
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button BackButton;

    [Header("Audio Settings")]
    [SerializeField] private Slider AudioSlider;
    [SerializeField] private TMP_Text AudioValueText;

    [Header("Music Settings")]
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private TMP_Text MusicValueText;

    [Header("Sensitivity Settings")]
    [SerializeField] private Slider SensitivitySlider;
    [SerializeField] private TMP_Text SensitivityValueText;

    private void Awake()
    {
        RegisterButtonEvents();
        RegisterSliderEvents();

        OptionsPanel.SetActive(false);
    }

    private void Start()
    {
        if (GameBootstrapper.Instance == null)
        {
            Debug.LogError("GameBootstrapper instance could not be found.");
            enabled = false;
            return;
        }

        gameSettingsService =
            GameBootstrapper.Instance.GameSettingsService;

        if (gameSettingsService == null)
        {
            Debug.LogError("GameSettingsService could not be found.");
            enabled = false;
        }
    }

    // =====================================================
    // INITIALIZATION
    // =====================================================

    private void RegisterButtonEvents()
    {
        OptionsButton.onClick.AddListener(OpenOptionsPanel);
        SaveButton.onClick.AddListener(SaveSettings);
        BackButton.onClick.AddListener(CloseOptionsPanel);
    }

    private void RegisterSliderEvents()
    {
        AudioSlider.onValueChanged.AddListener(UpdateAudioValueText);
        MusicSlider.onValueChanged.AddListener(UpdateMusicValueText);
        SensitivitySlider.onValueChanged.AddListener(UpdateSensitivityValueText);
    }

    // =====================================================
    // OPTIONS PANEL
    // =====================================================

    private void OpenOptionsPanel()
    {
        LoadSettings();

        OptionsPanel.SetActive(true);
    }

    private void CloseOptionsPanel()
    {
        OptionsPanel.SetActive(false);
    }

    // =====================================================
    // LOAD SETTINGS
    // =====================================================

    private void LoadSettings()
    {
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

        AudioSlider.value = settings.AudioLevel;
        MusicSlider.value = settings.MusicLevel;
        SensitivitySlider.value = settings.Sensitivity;
    }

    // =====================================================
    // SAVE SETTINGS
    // =====================================================

    private void SaveSettings()
    {
        bool result = gameSettingsService.UpdateSettings(
            AudioSlider.value,
            MusicSlider.value,
            SensitivitySlider.value
        );

        if (!result)
        {
            Debug.LogError("Game settings could not be saved.");
            return;
        }

        Debug.Log("Game settings saved successfully.");
    }

    // =====================================================
    // SLIDER VALUE TEXTS
    // =====================================================

    private void UpdateAudioValueText(float value)
    {
        AudioValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void UpdateMusicValueText(float value)
    {
        MusicValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void UpdateSensitivityValueText(float value)
    {
        SensitivityValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
