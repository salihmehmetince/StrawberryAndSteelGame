
using System;

public class PlayerProfileService
{
    private readonly DatabaseContext databaseContext;
    private readonly PlayerService playerService;
    private readonly PlayerProgressService progressService;
    private readonly PlayerEquipmentRepository playerEquipmentRepository;
    private readonly PlayerHasItemRepository playerHasItemRepository;
    private readonly PlayerCharacterStatsService characterStatsService;

    public PlayerProfileService(
        DatabaseContext databaseContext,
        PlayerService playerService,
        PlayerProgressService progressService,
        PlayerEquipmentRepository playerEquipmentRepository,
        PlayerHasItemRepository playerHasItemRepository,
        PlayerCharacterStatsService characterStatsService)
    {
        this.databaseContext = databaseContext;
        this.playerService = playerService;
        this.progressService = progressService;
        this.playerEquipmentRepository = playerEquipmentRepository;
        this.playerHasItemRepository = playerHasItemRepository;
        this.characterStatsService = characterStatsService;
    }

    public int CreatePlayerWithDefaults(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            throw new ArgumentException("Player name cannot be empty.");

        int playerId = 0;

        databaseContext.RunInTransaction(() =>
        {
            playerId = playerService.CreatePlayer(playerName);

            progressService.CreateProgress(playerId);

            characterStatsService.CreateStats(playerId);
        });

        return playerId;
    }

    public bool OverwritePlayerWithDefaults(int playerId, string playerName)
    {
        if (playerId <= 0 || string.IsNullOrWhiteSpace(playerName))
            return false;

        Player player = playerService.GetPlayerById(playerId);

        if (player == null)
            return false;

        bool result = false;

        databaseContext.RunInTransaction(() =>
        {
            playerEquipmentRepository.DeleteByPlayerId(playerId);

            playerHasItemRepository.DeleteByPlayerId(playerId);

            progressService.DeleteProgress(playerId);

            characterStatsService.DeleteStats(playerId);

            player.PlayerName = playerName.Trim();
            player.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (!playerService.UpdatePlayer(player))
                throw new InvalidOperationException("Player update failed.");

            progressService.CreateProgress(playerId);

            characterStatsService.CreateStats(playerId);

            result = true;
        });

        return result;
    }

    public bool EnsureCharacterStats(int playerId)
    {
        if (playerId <= 0)
            return false;

        if (characterStatsService.StatsExists(playerId))
            return true;

        return characterStatsService.CreateStats(playerId) > 0;
    }
}
