
using UnityEngine;

public class DiamondCollectionController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip collectionSound;

    private bool isCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || !other.CompareTag("Player"))
            return;

        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper == null ||
            bootstrapper.LevelCollectableSession == null)
        {
            Debug.LogError("Level collectable session not found.");
            return;
        }

        isCollected = true;

        bootstrapper.LevelCollectableSession.AddDiamond();

        CollectablePanelController.Instance?.RefreshCounts();

        PlayCollectionSound(bootstrapper);

        Destroy(gameObject);
    }

    private void PlayCollectionSound(GameBootstrapper bootstrapper)
    {
        if (collectionSound == null)
            return;

        GameSettings settings =
            bootstrapper.GameSettingsService.GetSettings();

        if (settings == null)
        {
            Debug.LogError("Game settings could not be loaded.");
            return;
        }

        AudioSource.PlayClipAtPoint(
            collectionSound,
            transform.position,
            Mathf.Clamp01(settings.AudioLevel)
        );
    }
}
