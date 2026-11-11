
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1PortalManager : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private GameObject portalButtonIcon;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem portalFlash;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [Header("Scene Settings")]
    [SerializeField] private string levelsSelectionSceneName = "LevelSelectionScene";

    private const int CompletedLevel = 1;

    private bool playerInRange;
    private bool isCompleting;

    private void Start()
    {
        if (portalButtonIcon != null)
            portalButtonIcon.SetActive(false);

        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper != null &&
            bootstrapper.LevelCollectableSession != null)
        {
            bootstrapper.LevelCollectableSession.Clear();
        }
    }

    private void Update()
    {
        if (!playerInRange || isCompleting || playerInputHandler == null)
            return;

        if (playerInputHandler.ConsumeInteract())
        {
            CompleteLevelAndReturn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || playerInRange)
            return;

        playerInRange = true;

        if (portalButtonIcon != null)
            portalButtonIcon.SetActive(true);

        PlayPortalSound();

        if (portalFlash != null)
        {
            portalFlash.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            portalFlash.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;

        if (portalButtonIcon != null)
            portalButtonIcon.SetActive(false);
    }

    private void PlayPortalSound()
    {
        if (audioSource == null)
            return;

        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper == null)
            return;

        GameSettings settings =
            bootstrapper.GameSettingsService.GetSettings();

        if (settings == null)
        {
            Debug.LogError("Game settings could not be loaded.");
            return;
        }

        audioSource.volume = Mathf.Clamp01(settings.AudioLevel);
        audioSource.Play();
    }

    private void CompleteLevelAndReturn()
    {
        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper == null ||
            !bootstrapper.GameSession.HasActivePlayer ||
            bootstrapper.LevelCollectableSession == null)
        {
            Debug.LogError("Game session could not be loaded.");
            return;
        }

        isCompleting = true;

        int playerId = bootstrapper.GameSession.CurrentPlayerId;

        int coinCount =
            bootstrapper.LevelCollectableSession.CoinCount;

        int diamondCount =
            bootstrapper.LevelCollectableSession.DiamondCount;

        int coinItemId = GetCollectableItemId(bootstrapper, "Coin");
        int diamondItemId = GetCollectableItemId(bootstrapper, "Diamond");

        if (coinCount > 0 && coinItemId <= 0)
        {
            Debug.LogError("Coin item not found.");
            isCompleting = false;
            return;
        }

        if (diamondCount > 0 && diamondItemId <= 0)
        {
            Debug.LogError("Diamond item not found.");
            isCompleting = false;
            return;
        }

        try
        {
            bootstrapper.DatabaseContext.Connection.RunInTransaction(() =>
            {
                if (coinCount > 0 &&
                    bootstrapper.PlayerHasItemService.AddItem(
                        playerId,
                        coinItemId,
                        coinCount) <= 0)
                {
                    throw new InvalidOperationException(
                        "Coin rewards could not be saved."
                    );
                }

                if (diamondCount > 0 &&
                    bootstrapper.PlayerHasItemService.AddItem(
                        playerId,
                        diamondItemId,
                        diamondCount) <= 0)
                {
                    throw new InvalidOperationException(
                        "Diamond rewards could not be saved."
                    );
                }

                if (!bootstrapper.PlayerProgressService.CompleteLevel(
                    playerId,
                    CompletedLevel))
                {
                    throw new InvalidOperationException(
                        "Level progress could not be saved."
                    );
                }
            });

            bootstrapper.LevelCollectableSession.Clear();

            SceneManager.LoadScene(levelsSelectionSceneName);
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Level completion failed. Database changes were rolled back. " +
                exception.Message
            );

            isCompleting = false;
        }
    }

    private int GetCollectableItemId(
        GameBootstrapper bootstrapper,
        string itemName)
    {
        ItemType itemType =
            bootstrapper.ItemTypeService.GetItemTypeByName("Collectable");

        if (itemType == null)
            return 0;

        List<Item> items =
            bootstrapper.ItemService.GetItemsByType(itemType.ItemTypeId);

        foreach (Item item in items)
        {
            if (item.ItemName == itemName)
                return item.ItemId;
        }

        return 0;
    }
}
