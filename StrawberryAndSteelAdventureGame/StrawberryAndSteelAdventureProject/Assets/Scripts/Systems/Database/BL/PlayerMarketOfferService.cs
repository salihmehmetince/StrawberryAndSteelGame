
using System;
using System.Collections.Generic;

public class PlayerMarketOfferService
{
    private readonly PlayerMarketOfferRepository repository;
    private readonly PlayerMarketRepository marketRepository;

    public PlayerMarketOfferService(
        PlayerMarketOfferRepository repository,
        PlayerMarketRepository marketRepository)
    {
        this.repository = repository ??
            throw new ArgumentNullException(nameof(repository));

        this.marketRepository = marketRepository ??
            throw new ArgumentNullException(nameof(marketRepository));
    }

    public int CreateOffer(PlayerMarketOffer offer)
    {
        if (offer == null)
            return 0;

        if (offer.PlayerMarketId <= 0)
            return 0;

        if (offer.SlotNumber < 1 || offer.SlotNumber > 6)
            return 0;

        if (offer.ItemId <= 0)
            return 0;

        if (offer.CoinPrice < 0 || offer.DiamondPrice < 0)
            return 0;

        if (offer.IsPurchased != 0 && offer.IsPurchased != 1)
            return 0;

        if (offer.IsPurchased == 1 &&
            string.IsNullOrWhiteSpace(offer.PurchasedAtUtc))
            return 0;

        if (marketRepository.GetById(offer.PlayerMarketId) == null)
            return 0;

        if (repository.ExistsInSlot(
            offer.PlayerMarketId,
            offer.SlotNumber))
            return 0;

        return repository.Add(offer);
    }

    public PlayerMarketOffer GetOfferById(int offerId)
    {
        if (offerId <= 0)
            return null;

        return repository.GetById(offerId);
    }

    public PlayerMarketOffer GetOfferBySlot(
        int playerMarketId,
        int slotNumber)
    {
        return repository.GetByMarketAndSlot(
            playerMarketId,
            slotNumber);
    }

    public List<PlayerMarketOffer> GetOffersByMarketId(int playerMarketId)
    {
        return repository.GetByMarketId(playerMarketId);
    }

    public bool MarkAsPurchased(int offerId, string purchasedAtUtc)
    {
        if (offerId <= 0 ||
            string.IsNullOrWhiteSpace(purchasedAtUtc))
            return false;

        PlayerMarketOffer offer = repository.GetById(offerId);

        if (offer == null || offer.IsPurchased == 1)
            return false;

        offer.IsPurchased = 1;
        offer.PurchasedAtUtc = purchasedAtUtc;

        return repository.Update(offer);
    }

    public int DeleteOffersByMarketId(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return 0;

        return repository.DeleteByMarketId(playerMarketId);
    }

    public int GetOfferCount(int playerMarketId)
    {
        if (playerMarketId <= 0)
            return 0;

        return repository.CountByMarketId(playerMarketId);
    }
}
