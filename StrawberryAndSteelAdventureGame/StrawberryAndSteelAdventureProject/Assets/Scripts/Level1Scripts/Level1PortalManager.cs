using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1PortalManager : MonoBehaviour
{
    [SerializeField] private GameObject portalButtonIcon;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem portalFlash;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private bool playerInRange;

    private void Start()
    {
        portalButtonIcon.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (playerInputHandler.ConsumeInteract())
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || playerInRange)
            return;

        playerInRange = true;
        portalButtonIcon.SetActive(true);

        if (audioSource != null)
            audioSource.Play();

        if (portalFlash != null)
        {
            portalFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            portalFlash.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;
        portalButtonIcon.SetActive(false);
    }
}