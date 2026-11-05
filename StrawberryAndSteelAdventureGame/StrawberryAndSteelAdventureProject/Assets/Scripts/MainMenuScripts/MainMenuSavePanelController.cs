
using System;
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
    [Header("Database")]
    [SerializeField] private GameDatabaseManager databaseManager;

    [Header("Main Menu")]
    [SerializeField] private Button NewGameButton;

    [Header("Save Panel")]
    [SerializeField] private GameObject GameSlotsPanelSave;
    [SerializeField] private GameSlotUI[] GameSlots = new GameSlotUI[3];
    [SerializeField] private Button GameSlotsCloseButton;

    [Header("New Player Panel")]
    [SerializeField] private GameObject NewPlayerPanel;
    [SerializeField] private TMP_InputField PlayerNameInput;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button NewPlayerCancelButton;

    [Header("Override Panel")]
    [SerializeField] private GameObject OverridePanel;
    [SerializeField] private Button OverrideButton;
    [SerializeField] private Button CancelButton;

    [Header("Scene")]
    [SerializeField] private string LoadingSceneName = "LoadingScene";

    private PlayerData[] slotPlayers = new PlayerData[3];

    private int selectedSlotIndex = -1;
    private int selectedPlayerId = 0;

    private void Awake()
    {
        InitializePanels();
        RegisterButtonEvents();
    }

    private void InitializePanels()
    {
        GameSlotsPanelSave.SetActive(false);
        NewPlayerPanel.SetActive(false);
        OverridePanel.SetActive(false);
    }

    private void RegisterButtonEvents()
    {
        NewGameButton.onClick.AddListener(OpenSavePanel);

        GameSlotsCloseButton.onClick.AddListener(CloseSavePanel);

        for (int i = 0; i < GameSlots.Length; i++)
        {
            int slotIndex = i;

            GameSlots[i].GameSlotButton.onClick.AddListener(
                () => SelectSlot(slotIndex)
            );
        }

        ContinueButton.onClick.AddListener(CreatePlayer);

        NewPlayerCancelButton.onClick.AddListener(CancelNewPlayer);

        OverrideButton.onClick.AddListener(OpenNewPlayerPanel);

        CancelButton.onClick.AddListener(CloseOverridePanel);
    }

    private void OpenSavePanel()
    {
        RefreshSlots();

        GameSlotsPanelSave.SetActive(true);
        NewPlayerPanel.SetActive(false);
        OverridePanel.SetActive(false);

        ResetSelection();
    }

    private void CloseSavePanel()
    {
        GameSlotsPanelSave.SetActive(false);
        NewPlayerPanel.SetActive(false);
        OverridePanel.SetActive(false);

        ResetSelection();
    }

    private void RefreshSlots()
    {
        var players = databaseManager.GetAllPlayers();

        for (int i = 0; i < GameSlots.Length; i++)
        {
            PlayerData player = i < players.Count
                ? players[i]
                : null;

            slotPlayers[i] = player;

            GameSlots[i].PlayerText.text = player == null
                ? "New Game"
                : player.PlayerName;

            GameSlots[i].GameSlotButton.interactable = true;
        }
    }

    private void SelectSlot(int slotIndex)
    {
        RefreshSlots();

        selectedSlotIndex = slotIndex;

        PlayerData selectedPlayer = slotPlayers[slotIndex];

        NewPlayerPanel.SetActive(false);
        OverridePanel.SetActive(false);

        if (selectedPlayer == null)
        {
            selectedPlayerId = 0;
            OpenNewPlayerPanel();
        }
        else
        {
            selectedPlayerId = selectedPlayer.PlayerId;
            OverridePanel.SetActive(true);
        }
    }

    private void OpenNewPlayerPanel()
    {
        OverridePanel.SetActive(false);

        PlayerNameInput.text = string.Empty;
        NewPlayerPanel.SetActive(true);

        PlayerNameInput.ActivateInputField();
    }

    private void CreatePlayer()
    {
        string playerName = PlayerNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("Please enter a player name.");
            PlayerNameInput.ActivateInputField();
            return;
        }

        if (selectedSlotIndex < 0 || selectedSlotIndex >= 3)
        {
            Debug.LogError("No valid save slot selected.");
            return;
        }

        try
        {
            if (selectedPlayerId > 0)
            {
                databaseManager.OverwritePlayerWithDefaults(
                    selectedPlayerId,
                    playerName
                );
            }
            else
            {
                databaseManager.CreatePlayerWithDefaults(
                    playerName
                );
            }

            Debug.Log("Player saved successfully: " + playerName);

            SceneManager.LoadScene(LoadingSceneName);
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "Player save failed: " + exception.Message
            );
        }
    }

    private void CancelNewPlayer()
    {
        NewPlayerPanel.SetActive(false);
        PlayerNameInput.text = string.Empty;

        if (selectedPlayerId > 0)
        {
            // Return to the override confirmation panel.
            OverridePanel.SetActive(true);
        }
        else
        {
            // Return to the save slots panel.
            ResetSelection();
        }
    }

    private void CloseOverridePanel()
    {
        OverridePanel.SetActive(false);
        ResetSelection();
    }

    private void ResetSelection()
    {
        selectedSlotIndex = -1;
        selectedPlayerId = 0;
    }
}
