using System.Collections.Generic;

public class PlayerService
{
    private readonly PlayerRepository playerRepository;

    public PlayerService(PlayerRepository playerRepository)
    {
        this.playerRepository = playerRepository;
    }

    public int CreatePlayer(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            throw new System.ArgumentException("Player name cannot be empty.");

        Player player = new Player
        {
            PlayerName = playerName.Trim(),
            CreatedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return playerRepository.Add(player);
    }

    public Player GetPlayerById(int playerId)
    {
        if (playerId <= 0)
            return null;

        return playerRepository.GetById(playerId);
    }

    public List<Player> GetAllPlayers()
    {
        return playerRepository.GetAll();
    }

    public bool UpdatePlayer(Player player)
    {
        if (player == null)
            return false;

        if (string.IsNullOrWhiteSpace(player.PlayerName))
            throw new System.ArgumentException("Player name cannot be empty.");

        player.PlayerName = player.PlayerName.Trim();

        return playerRepository.Update(player);
    }

    public bool DeletePlayer(int playerId)
    {
        if (playerId <= 0)
            return false;

        return playerRepository.Delete(playerId);
    }

    public bool PlayerExists(int playerId)
    {
        if (playerId <= 0)
            return false;

        return playerRepository.Exists(playerId);
    }

    public int GetPlayerCount()
    {
        return playerRepository.Count();
    }
}