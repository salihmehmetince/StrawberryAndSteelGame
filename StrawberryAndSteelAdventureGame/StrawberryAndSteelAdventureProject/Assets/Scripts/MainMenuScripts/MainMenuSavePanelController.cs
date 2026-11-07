
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class GameSlotUI
{
    public Button GameSlotButton;
    public TMP_Text PlayerText;
}

public class MainMenuSavePanelController : MonoBehaviour
{
    private enum SlotSelectionMode
    {
        None,
        Save,
        Load
    }

    [Header("BL Services")]
    private PlayerService playerService;
    private PlayerProfileService playerProfileService;

    [Header("Game Session")]
    private GameSession gameSession;

    [Header("Main Menu")]
    [SerializeField] private Button NewGameButton;
    [SerializeField] private Button LoadGameButton;

    [Header("Save Panel")]
    [SerializeField] private GameObject GameSlotsPanelSave;
    [SerializeField] private GameSlotUI[] GameSlotsSave = new GameSlotUI[3];
    [SerializeField] private Button GameSlotsCloseButton;

    [Header("Save - New Player Panel")]
    [SerializeField] private GameObject NewPlayerPanelSave;
    [SerializeField] private TMP_InputField PlayerNameInputSave;
    [SerializeField] private Button ContinueButtonSave;
    [SerializeField] private Button CancelButtonSave;

    [Header("Override Panel")]
    [SerializeField] private GameObject OverridePanel;
    [SerializeField] private Button OverrideButton;
    [SerializeField] private Button OverrideCancelButton;

    [Header("Load Panel")]
    [SerializeField] private GameObject GameSlotsPanelLoad;
    [SerializeField] private GameSlotUI[] GameSlotsLoad = new GameSlotUI[3];
    [SerializeField] private Button GameSlotsLoadCloseButton;

    [Header("Load - New Player Panel")]
    [SerializeField] private GameObject NewPlayerPanelLoad;
    [SerializeField] private TMP_InputField PlayerNameInputLoad;
    [SerializeField] private Button ContinueButtonLoad;
    [SerializeField] private Button CancelButtonLoad;

    [Header("Scene")]
    [SerializeField] private string LoadingSceneName = "LoadingScene";

    private Player[] slotPlayers;

    private int selectedSlotIndex = -1;
    private int selectedPlayerId = 0;

    private SlotSelectionMode selectionMode = SlotSelectionMode.None;

    // =====================================================
    // INITIALIZATION
    // =====================================================

    private void Awake()
    {
        InitializePanels();
        RegisterButtonEvents();
    }

    private void Start()
    {
        if (GameBootstrapper.Instance == null)
        {
            Debug.LogError("GameBootstrapper instance could not be found.");
            enabled = false;
            return;
        }

        playerService = GameBootstrapper.Instance.PlayerService;
        playerProfileService = GameBootstrapper.Instance.PlayerProfileService;
        gameSession = GameBootstrapper.Instance.GameSession;

        if (playerService == null ||
            playerProfileService == null ||
            gameSession == null)
        {
            Debug.LogError("Required services or GameSession could not be initialized.");
            enabled = false;
            return;
        }
    }

    private void InitializePanels()
    {
        if (GameSlotsPanelSave != null)
            GameSlotsPanelSave.SetActive(false);

        if (GameSlotsPanelLoad != null)
            GameSlotsPanelLoad.SetActive(false);

        if (NewPlayerPanelSave != null)
            NewPlayerPanelSave.SetActive(false);

        if (NewPlayerPanelLoad != null)
            NewPlayerPanelLoad.SetActive(false);

        if (OverridePanel != null)
            OverridePanel.SetActive(false);
    }

    private void RegisterButtonEvents()
    {
        if (NewGameButton != null)
            NewGameButton.onClick.AddListener(OpenSavePanel);

        if (LoadGameButton != null)
            LoadGameButton.onClick.AddListener(OpenLoadPanel);

        if (GameSlotsCloseButton != null)
            GameSlotsCloseButton.onClick.AddListener(CloseSavePanel);

        if (GameSlotsLoadCloseButton != null)
            GameSlotsLoadCloseButton.onClick.AddListener(CloseLoadPanel);

        RegisterSlotButtons(GameSlotsSave, SlotSelectionMode.Save);
        RegisterSlotButtons(GameSlotsLoad, SlotSelectionMode.Load);

        if (ContinueButtonSave != null)
        {
            ContinueButtonSave.onClick.AddListener(
                () => CreatePlayer(SlotSelectionMode.Save)
            );
        }

        if (CancelButtonSave != null)
            CancelButtonSave.onClick.AddListener(CancelNewPlayerSave);

        if (OverrideButton != null)
            OverrideButton.onClick.AddListener(OpenNewPlayerSave);

        if (OverrideCancelButton != null)
            OverrideCancelButton.onClick.AddListener(CancelOverridePanel);

        if (ContinueButtonLoad != null)
        {
            ContinueButtonLoad.onClick.AddListener(
                () => CreatePlayer(SlotSelectionMode.Load)
            );
        }

        if (CancelButtonLoad != null)
            CancelButtonLoad.onClick.AddListener(CancelNewPlayerLoad);
    }

    private void RegisterSlotButtons(
        GameSlotUI[] slots,
        SlotSelectionMode mode)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null ||
                slots[i].GameSlotButton == null)
                continue;

            int slotIndex = i;

            slots[i].GameSlotButton.onClick.AddListener(
                () => SelectSlot(slotIndex, mode)
            );
        }
    }

    // =====================================================
    // SAVE PANEL
    // =====================================================

    public void OpenSavePanel()
    {
        if (playerService == null)
            return;

        ResetSelection();

        selectionMode = SlotSelectionMode.Save;

        RefreshSlots(GameSlotsSave);

        GameSlotsPanelLoad.SetActive(false);
        GameSlotsPanelSave.SetActive(true);

        NewPlayerPanelSave.SetActive(false);
        OverridePanel.SetActive(false);
    }

    private void CloseSavePanel()
    {
        GameSlotsPanelSave.SetActive(false);

        ResetSelection();
        selectionMode = SlotSelectionMode.None;
    }

    // =====================================================
    // LOAD PANEL
    // =====================================================

    public void OpenLoadPanel()
    {
        if (playerService == null)
            return;

        ResetSelection();

        selectionMode = SlotSelectionMode.Load;

        RefreshSlots(GameSlotsLoad);

        GameSlotsPanelSave.SetActive(false);
        GameSlotsPanelLoad.SetActive(true);

        NewPlayerPanelLoad.SetActive(false);
    }

    private void CloseLoadPanel()
    {
        GameSlotsPanelLoad.SetActive(false);

        ResetSelection();
        selectionMode = SlotSelectionMode.None;
    }

    // =====================================================
    // SLOT MANAGEMENT
    // =====================================================

    private void RefreshSlots(GameSlotUI[] slots)
    {
        List<Player> players = playerService.GetAllPlayers();

        slotPlayers = new Player[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            Player player = i < players.Count
                ? players[i]
                : null;

            slotPlayers[i] = player;

            if (slots[i] == null)
                continue;

            if (slots[i].PlayerText != null)
            {
                slots[i].PlayerText.text = player == null
                    ? "New Game"
                    : player.PlayerName;
            }

            if (slots[i].GameSlotButton != null)
                slots[i].GameSlotButton.interactable = true;
        }
    }

    private void SelectSlot(
        int slotIndex,
        SlotSelectionMode mode)
    {
        if (mode != selectionMode || slotIndex < 0)
            return;

        GameSlotUI[] slots = GetCurrentSlots();

        if (slots == null || slotIndex >= slots.Length)
            return;

        RefreshSlots(slots);

        selectedSlotIndex = slotIndex;

        Player selectedPlayer = slotPlayers[slotIndex];

        if (selectionMode == SlotSelectionMode.Save)
        {
            NewPlayerPanelSave.SetActive(false);
            OverridePanel.SetActive(false);
        }
        else
        {
            NewPlayerPanelLoad.SetActive(false);
        }

        if (selectedPlayer == null)
        {
            selectedPlayerId = 0;

            if (selectionMode == SlotSelectionMode.Save)
                OpenNewPlayerSave();
            else
                OpenNewPlayerLoad();

            return;
        }

        selectedPlayerId = selectedPlayer.PlayerId;

        if (selectionMode == SlotSelectionMode.Save)
        {
            OverridePanel.SetActive(true);
        }
        else if (selectionMode == SlotSelectionMode.Load)
        {
            LoadPlayer(selectedPlayer.PlayerId);
        }
    }

    private GameSlotUI[] GetCurrentSlots()
    {
        if (selectionMode == SlotSelectionMode.Save)
            return GameSlotsSave;

        if (selectionMode == SlotSelectionMode.Load)
            return GameSlotsLoad;

        return null;
    }

    // =====================================================
    // SAVE - NEW PLAYER
    // =====================================================

    private void OpenNewPlayerSave()
    {
        OverridePanel.SetActive(false);

        PlayerNameInputSave.text = string.Empty;

        NewPlayerPanelSave.SetActive(true);

        PlayerNameInputSave.ActivateInputField();
    }

    private void CancelNewPlayerSave()
    {
        NewPlayerPanelSave.SetActive(false);

        PlayerNameInputSave.text = string.Empty;

        if (selectedPlayerId > 0)
        {
            OverridePanel.SetActive(true);
        }
    }

    // =====================================================
    // LOAD - NEW PLAYER
    // =====================================================

    private void OpenNewPlayerLoad()
    {
        PlayerNameInputLoad.text = string.Empty;

        NewPlayerPanelLoad.SetActive(true);

        PlayerNameInputLoad.ActivateInputField();
    }

    private void CancelNewPlayerLoad()
    {
        NewPlayerPanelLoad.SetActive(false);

        PlayerNameInputLoad.text = string.Empty;
    }

    // =====================================================
    // PLAYER CREATION / OVERWRITE
    // =====================================================

    private void CreatePlayer(SlotSelectionMode mode)
    {
        if (mode != selectionMode)
            return;

        TMP_InputField nameInput = mode == SlotSelectionMode.Save
            ? PlayerNameInputSave
            : PlayerNameInputLoad;

        string playerName = nameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("Player name cannot be empty.");

            nameInput.ActivateInputField();
            return;
        }

        GameSlotUI[] slots = GetCurrentSlots();

        if (slots == null ||
            selectedSlotIndex < 0 ||
            selectedSlotIndex >= slots.Length)
        {
            Debug.LogError("No valid save slot selected.");
            return;
        }

        try
        {
            int activePlayerId;

            if (mode == SlotSelectionMode.Save && selectedPlayerId > 0)
            {
                bool result = playerProfileService.OverwritePlayerWithDefaults(
                    selectedPlayerId,
                    playerName
                );

                if (!result)
                {
                    Debug.LogError("Player overwrite operation failed.");
                    return;
                }

                activePlayerId = selectedPlayerId;
            }
            else
            {
                activePlayerId = playerProfileService.CreatePlayerWithDefaults(
                    playerName
                );

                if (activePlayerId <= 0)
                {
                    Debug.LogError("Player creation operation failed.");
                    return;
                }
            }

            gameSession.SetCurrentPlayer(activePlayerId);

            Debug.Log(
                "Player saved successfully. Player ID: " + activePlayerId
            );

            SceneTransitionManager.Instance.LoadScene("LevelSelectionScene");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Player save operation failed: " + exception.Message
            );
        }
    }

    // =====================================================
    // LOAD PLAYER
    // =====================================================

    private void LoadPlayer(int playerId)
    {
        if (playerId <= 0)
        {
            Debug.LogError("Invalid PlayerId.");
            return;
        }

        try
        {
            gameSession.SetCurrentPlayer(playerId);

            Debug.Log("Player loaded. Player ID: " + playerId);

            SceneTransitionManager.Instance.LoadScene("LevelSelectionScene");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Player loading failed: " + exception.Message
            );
        }
    }

    // =====================================================
    // OVERRIDE PANEL
    // =====================================================

    private void CancelOverridePanel()
    {
        OverridePanel.SetActive(false);

        ResetSelection();
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private void ResetSelection()
    {
        selectedSlotIndex = -1;
        selectedPlayerId = 0;
        slotPlayers = null;
    }
}
