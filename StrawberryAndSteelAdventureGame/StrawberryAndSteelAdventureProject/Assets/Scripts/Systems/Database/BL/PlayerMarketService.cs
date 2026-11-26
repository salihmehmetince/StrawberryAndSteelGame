
using System;

public class PlayerMarketService
{
    private readonly PlayerMarketRepository repository;

    public PlayerMarketService(PlayerMarketRepository repository)
    {
        this.repository = repository ??
            throw new ArgumentNullException(nameof(repository));
    }

    public int CreateMarket(PlayerMarket market)
    {
        if (market == null)
            return 0;

        if (market.PlayerId <= 0)
            return 0;

        if (string.IsNullOrWhiteSpace(market.CreatedAtUtc))
            return 0;

        if (string.IsNullOrWhiteSpace(market.NextRefreshAtUtc))
            return 0;

        if (repository.Exists(market.PlayerId))
            return 0;

        return repository.Add(market);
    }

    public PlayerMarket GetMarketById(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return null;

        return repository.GetById(playerMarketId);
    }

    public PlayerMarket GetMarketByPlayerId(int playerId)
    {
        if (playerId <= 0)
            return null;

        return repository.GetByPlayerId(playerId);
    }

    public bool UpdateMarket(PlayerMarket market)
    {
        if (market == null || market.PlayerMarketId <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(market.NextRefreshAtUtc))
            return false;

        return repository.Update(market);
    }

    public bool DeleteMarket(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return false;

        return repository.Delete(playerMarketId);
    }

    public int GetMarketCount()
    {
        return repository.Count();
    }
}
