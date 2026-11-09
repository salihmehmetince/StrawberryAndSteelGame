
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelSelectionButtonController : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelNumber;

    private Button levelButton;

    private void Awake()
    {
        levelButton = GetComponent<Button>();
        levelButton.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        string sceneName = "Act" + levelNumber + "Scene";

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("SceneTransitionManager bulunamadý.");
            return;
        }

        SceneTransitionManager.Instance.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        if (levelButton != null)
            levelButton.onClick.RemoveListener(LoadLevel);
    }
}
