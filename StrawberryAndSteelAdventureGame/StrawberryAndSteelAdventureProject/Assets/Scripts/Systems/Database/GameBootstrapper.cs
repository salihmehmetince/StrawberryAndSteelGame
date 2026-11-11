
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

    public DatabaseContext DatabaseContext { get; private set; }

    public GameSession GameSession { get; private set; }

    public PlayerService PlayerService { get; private set; }
    public PlayerProgressService PlayerProgressService { get; private set; }
    public GameSettingsService GameSettingsService { get; private set; }
    public PlayerHasItemService PlayerHasItemService { get; private set; }
    public PlayerEquipmentService PlayerEquipmentService { get; private set; }
    public ItemService ItemService { get; private set; }
    public ItemTypeService ItemTypeService { get; private set; }
    public PlayerProfileService PlayerProfileService { get; private set; }

    public LevelCollectableSession LevelCollectableSession { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSession();
        InitializeDatabase();
        InitializeRepositoriesAndServices();
    }

    private void InitializeSession()
    {
        GameSession = new GameSession();
        LevelCollectableSession = new LevelCollectableSession();

    }

    private void InitializeDatabase()
    {
        DatabaseContext = new DatabaseContext("StrawberryAndSteel.db");
        DatabaseInitializer.Initialize(DatabaseContext);
    }

    private void InitializeRepositoriesAndServices()
    {
        PlayerRepository playerRepository =
            new PlayerRepository(DatabaseContext);

        PlayerProgressRepository progressRepository =
            new PlayerProgressRepository(DatabaseContext);

        GameSettingsRepository gameSettingsRepository =
            new GameSettingsRepository(DatabaseContext);

        PlayerHasItemRepository playerHasItemRepository =
            new PlayerHasItemRepository(DatabaseContext);

        PlayerEquipmentRepository equipmentRepository =
            new PlayerEquipmentRepository(DatabaseContext);

        ItemRepository itemRepository =
            new ItemRepository(DatabaseContext);

        ItemTypeRepository itemTypeRepository =
            new ItemTypeRepository(DatabaseContext);

        PlayerService = new PlayerService(playerRepository);

        PlayerProgressService =
            new PlayerProgressService(progressRepository);

        GameSettingsService =
            new GameSettingsService(gameSettingsRepository);

        PlayerHasItemService =
            new PlayerHasItemService(
                playerHasItemRepository,
                itemRepository
            );

        PlayerEquipmentService =
            new PlayerEquipmentService(
                equipmentRepository,
                playerHasItemRepository
            );

        ItemService = new ItemService(itemRepository);

        ItemTypeService = new ItemTypeService(itemTypeRepository);

        PlayerProfileService = new PlayerProfileService(
            DatabaseContext,
            PlayerService,
            PlayerProgressService,
            equipmentRepository,
            playerHasItemRepository
        );

        if (!GameSettingsService.EnsureDefaultSettings())
        {
            Debug.LogError("Game settings initialization failed.");
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        DatabaseContext?.Dispose();
        Instance = null;
    }
}
