
using UnityEngine;
using UnityEngine.UI;

public class LevelsSelectionSceneManager : MonoBehaviour
{
    [System.Serializable]
    private class LevelEntry
    {
        public Button LevelButton;
        public GameObject LevelGlowUI;
        public LevelsSelectionBackgroundChain BackgroundChain;
    }

    [Header("Level Buttons")]
    [SerializeField] private LevelEntry[] levels;

    private void Start()
    {
        LoadPlayerProgress();
    }

    private void LoadPlayerProgress()
    {
        if (GameBootstrapper.Instance == null)
        {
            Debug.LogError("GameBootstrapper bulunamadý.");
            return;
        }

        GameSession gameSession = GameBootstrapper.Instance.GameSession;

        if (gameSession == null || !gameSession.HasActivePlayer)
        {
            Debug.LogError("Aktif oyuncu oturumu bulunamadý.");
            return;
        }

        PlayerProgressService progressService =
            GameBootstrapper.Instance.PlayerProgressService;

        if (progressService == null)
        {
            Debug.LogError("PlayerProgressService bulunamadý.");
            return;
        }

        PlayerProgress progress =
            progressService.GetProgress(gameSession.CurrentPlayerId);

        if (progress == null)
        {
            Debug.LogError("Oyuncu ilerleme bilgisi bulunamadý.");
            return;
        }

        UpdateLevelButtons(progress.LastUnlockedLevel);
    }

    private void UpdateLevelButtons(int lastUnlockedLevel)
    {
        if (levels == null || levels.Length != 10)
        {
            Debug.LogError("Tam olarak 10 seviye tanýmlanmalýdýr.");
            return;
        }

        if (lastUnlockedLevel < 1 || lastUnlockedLevel > levels.Length)
        {
            Debug.LogError("LastUnlockedLevel deðeri geçersiz.");
            return;
        }

        for (int i = 0; i < levels.Length; i++)
        {
            int levelNumber = i + 1;
            LevelEntry level = levels[i];

            if (level.LevelButton == null ||
                level.LevelGlowUI == null ||
                level.BackgroundChain == null)
            {
                Debug.LogError("Level " + levelNumber + " referanslarý eksik.");
                continue;
            }

            // Kilit kontrolü
            level.LevelButton.interactable =
                levelNumber <= lastUnlockedLevel;

            if (levelNumber < lastUnlockedLevel)
            {
                level.LevelGlowUI.SetActive(false);
                level.BackgroundChain.gameObject.SetActive(false);
            }
            else if (levelNumber == lastUnlockedLevel)
            {
                level.LevelGlowUI.SetActive(true);
                level.BackgroundChain.gameObject.SetActive(true);

                level.BackgroundChain.PlayBreakAnimation();
            }
            else
            {
                level.LevelGlowUI.SetActive(false);
                level.BackgroundChain.gameObject.SetActive(true);
            }
        }
    }
}
