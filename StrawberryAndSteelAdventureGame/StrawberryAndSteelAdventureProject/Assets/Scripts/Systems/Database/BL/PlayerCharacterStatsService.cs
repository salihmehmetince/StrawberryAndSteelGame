
using System;

public class PlayerCharacterStatsService
{
    private readonly PlayerCharacterStatsRepository statsRepository;

    public PlayerCharacterStatsService(
        PlayerCharacterStatsRepository statsRepository)
    {
        this.statsRepository = statsRepository;
    }

    public PlayerCharacterStats GetStats(int playerId)
    {
        if (playerId <= 0)
            return null;

        return statsRepository.GetByPlayerId(playerId);
    }

    public int CreateStats(int playerId)
    {
        if (playerId <= 0)
        {
            UnityEngine.Debug.LogError(
                "CreateStats: Invalid PlayerId: " + playerId
            );
            return 0;
        }

        if (statsRepository.Exists(playerId))
        {
            UnityEngine.Debug.LogError(
                "CreateStats: Stats already exist. PlayerId: " + playerId
            );
            return 0;
        }

        PlayerCharacterStats stats = new PlayerCharacterStats
        {
            PlayerId = playerId,
            TotalXP = 0
        };

        try
        {
            int result = statsRepository.Add(stats);

            UnityEngine.Debug.Log(
                "CreateStats: Insert result = " + result +
                ", PlayerId = " + playerId
            );

            return result;
        }
        catch (Exception exception)
        {
            UnityEngine.Debug.LogError(
                "CreateStats database error: " + exception
            );

            return 0;
        }
    }
    public bool AddXP(int playerId, long amount)
    {
        if (playerId <= 0 || amount <= 0)
            return false;

        PlayerCharacterStats stats =
            statsRepository.GetByPlayerId(playerId);

        if (stats == null)
            return false;

        try
        {
            stats.TotalXP = checked(stats.TotalXP + amount);
        }
        catch (OverflowException)
        {
            return false;
        }

        return statsRepository.Update(stats);
    }

    public bool SetTotalXP(int playerId, long totalXP)
    {
        if (playerId <= 0 || totalXP < 0)
            return false;

        PlayerCharacterStats stats =
            statsRepository.GetByPlayerId(playerId);

        if (stats == null)
            return false;

        stats.TotalXP = totalXP;

        return statsRepository.Update(stats);
    }

    public bool DeleteStats(int playerId)
    {
        if (playerId <= 0)
            return false;

        return statsRepository.DeleteByPlayerId(playerId);
    }

    public bool StatsExists(int playerId)
    {
        if (playerId <= 0)
            return false;

        return statsRepository.Exists(playerId);
    }
}
