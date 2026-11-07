
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
        // Loading Scene'in ilk karesinin görüntülenmesine izin ver.
        yield return null;

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

        // Her 5 saniyede bir sahnenin yüklenme durumunu kontrol et.
        while (operation.progress < 0.9f)
        {
            yield return new WaitForSecondsRealtime(LoadingIntervalSeconds);
        }

        // Sahne yüklenmeye hazýr. Aktivasyona izin ver.
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        isTransitioning = false;
    }
}
