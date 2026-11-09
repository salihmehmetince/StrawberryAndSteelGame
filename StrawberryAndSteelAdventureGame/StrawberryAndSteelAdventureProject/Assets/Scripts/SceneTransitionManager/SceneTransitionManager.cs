
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Loading Scene")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    private const float LoadingIntervalSeconds = 5f;

    private bool isTransitioning;

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

    public void LoadScene(string targetSceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Bir sahne geçiþi zaten devam ediyor.");
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError("Hedef sahne adý boþ olamaz.");
            return;
        }

        if (targetSceneName == loadingSceneName)
        {
            Debug.LogError("Loading Scene hedef sahne olarak seçilemez.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(loadingSceneName))
        {
            Debug.LogError("Loading Scene Build Settings içerisinde bulunamadý.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError("Hedef sahne Build Settings içerisinde bulunamadý: " + targetSceneName);
            return;
        }

        isTransitioning = true;

        SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Single);

        StartCoroutine(LoadTargetScene(targetSceneName));
    }

    private IEnumerator LoadTargetScene(string targetSceneName)
    {
        yield return null;

        LoadingSceneController loadingController =
            FindFirstObjectByType<LoadingSceneController>();

        if (loadingController == null)
        {
            Debug.LogError("LoadingSceneController bulunamadý.");
            isTransitioning = false;
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(
            targetSceneName,
            LoadSceneMode.Single
        );

        if (operation == null)
        {
            Debug.LogError("Hedef sahne yükleme iþlemi baþlatýlamadý.");
            isTransitioning = false;
            yield break;
        }

        operation.allowSceneActivation = false;

        float nextCheckTime =
            Time.realtimeSinceStartup + LoadingIntervalSeconds;

        while (true)
        {
            yield return null;

            if (Time.realtimeSinceStartup < nextCheckTime)
                continue;

            if (operation.progress >= 0.9f)
                break;

            nextCheckTime += LoadingIntervalSeconds;
        }

        loadingController.PlayExitAnimation();

        yield return new WaitUntil(() => loadingController.IsActionFinished);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        isTransitioning = false;
    }
}
