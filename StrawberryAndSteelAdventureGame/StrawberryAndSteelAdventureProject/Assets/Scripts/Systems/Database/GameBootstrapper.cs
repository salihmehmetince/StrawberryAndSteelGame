
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }

    public DatabaseContext DatabaseContext { get; private set; }

    public GameSession GameSession { get; private set; }

    public LevelCollectableSession LevelCollectableSession { get; private set; }

    // Services
    public PlayerService PlayerService { get; private set; }
    public PlayerProgressService PlayerProgressService { get; private set; }
    public GameSettingsService GameSettingsService { get; private set; }
    public PlayerCharacterStatsService PlayerCharacterStatsService { get; private set; }
    public PlayerHasItemService PlayerHasItemService { get; private set; }
    public PlayerEquipmentService PlayerEquipmentService { get; private set; }
    public ItemService ItemService { get; private set; }
    public ItemTypeService ItemTypeService { get; private set; }
    public ItemStatModifierService ItemStatModifierService { get; private set; }
    public SpellService SpellService { get; private set; }
    public SpellEffectService SpellEffectService { get; private set; }
    public PlayerHasSpellService PlayerHasSpellService { get; private set; }
    public PlayerActiveSpellService PlayerActiveSpellService { get; private set; }
    public CharacterLevelDefinitionService CharacterLevelDefinitionService { get; private set; }
    public PlayerLevelCompletionService PlayerLevelCompletionService { get; private set; }
    public EquipmentSlotService EquipmentSlotService { get; private set; }
    public ShopOfferService ShopOfferService { get; private set; }
    public DatabaseSchemaVersionService DatabaseSchemaVersionService { get; private set; }
    public PlayerProfileService PlayerProfileService { get; private set; }

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
        // Repositories

        PlayerRepository playerRepository =
            new PlayerRepository(DatabaseContext);

        PlayerProgressRepository progressRepository =
            new PlayerProgressRepository(DatabaseContext);

        GameSettingsRepository gameSettingsRepository =
            new GameSettingsRepository(DatabaseContext);

        PlayerCharacterStatsRepository characterStatsRepository =
            new PlayerCharacterStatsRepository(DatabaseContext);

        PlayerHasItemRepository playerHasItemRepository =
            new PlayerHasItemRepository(DatabaseContext);

        PlayerEquipmentRepository equipmentRepository =
            new PlayerEquipmentRepository(DatabaseContext);

        ItemRepository itemRepository =
            new ItemRepository(DatabaseContext);

        ItemTypeRepository itemTypeRepository =
            new ItemTypeRepository(DatabaseContext);

        ItemStatModifierRepository itemStatModifierRepository =
            new ItemStatModifierRepository(DatabaseContext);

        SpellRepository spellRepository =
            new SpellRepository(DatabaseContext);

        SpellEffectRepository spellEffectRepository =
            new SpellEffectRepository(DatabaseContext);

        PlayerHasSpellRepository playerHasSpellRepository =
            new PlayerHasSpellRepository(DatabaseContext);

        PlayerActiveSpellRepository playerActiveSpellRepository =
            new PlayerActiveSpellRepository(DatabaseContext);

        CharacterLevelDefinitionRepository characterLevelDefinitionRepository =
            new CharacterLevelDefinitionRepository(DatabaseContext);

        PlayerLevelCompletionRepository playerLevelCompletionRepository =
            new PlayerLevelCompletionRepository(DatabaseContext);

        EquipmentSlotRepository equipmentSlotRepository =
            new EquipmentSlotRepository(DatabaseContext);

        ShopOfferRepository shopOfferRepository =
            new ShopOfferRepository(DatabaseContext);

        DatabaseSchemaVersionRepository schemaVersionRepository =
            new DatabaseSchemaVersionRepository(DatabaseContext);


        // Services

        PlayerService = new PlayerService(playerRepository);

        PlayerProgressService =
            new PlayerProgressService(progressRepository);

        GameSettingsService =
            new GameSettingsService(gameSettingsRepository);

        PlayerCharacterStatsService =
            new PlayerCharacterStatsService(characterStatsRepository);

        PlayerHasItemService =
            new PlayerHasItemService(playerHasItemRepository);

        PlayerEquipmentService =
            new PlayerEquipmentService(
                equipmentRepository,
                playerHasItemRepository,
                itemRepository
            );

        ItemService = new ItemService(itemRepository);

        ItemTypeService = new ItemTypeService(itemTypeRepository);

        ItemStatModifierService =
            new ItemStatModifierService(itemStatModifierRepository);

        SpellService = new SpellService(spellRepository);

        SpellEffectService =
            new SpellEffectService(spellEffectRepository);

        PlayerHasSpellService =
            new PlayerHasSpellService(playerHasSpellRepository);

        PlayerActiveSpellService =
            new PlayerActiveSpellService(
                playerActiveSpellRepository,
                PlayerHasSpellService
            );

        CharacterLevelDefinitionService =
            new CharacterLevelDefinitionService(
                characterLevelDefinitionRepository
            );

        PlayerLevelCompletionService =
            new PlayerLevelCompletionService(
                playerLevelCompletionRepository
            );

        EquipmentSlotService =
            new EquipmentSlotService(equipmentSlotRepository);

        ShopOfferService =
            new ShopOfferService(shopOfferRepository);

        DatabaseSchemaVersionService =
            new DatabaseSchemaVersionService(schemaVersionRepository);


        // Player Profile

        PlayerProfileService = new PlayerProfileService(
            DatabaseContext,
            PlayerService,
            PlayerProgressService,
            equipmentRepository,
            playerHasItemRepository
        );


        // Default Settings

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
