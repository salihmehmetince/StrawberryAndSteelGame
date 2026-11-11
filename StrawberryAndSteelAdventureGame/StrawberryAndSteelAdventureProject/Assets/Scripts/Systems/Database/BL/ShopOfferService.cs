using System.Collections.Generic;

public class ShopOfferService
{
    private readonly ShopOfferRepository shopOfferRepository;

    public ShopOfferService(ShopOfferRepository shopOfferRepository)
    {
        this.shopOfferRepository = shopOfferRepository;
    }

    public ShopOffer GetOfferById(int shopOfferId)
    {
        if (shopOfferId <= 0)
            return null;

        return shopOfferRepository.GetById(shopOfferId);
    }

    public List<ShopOffer> GetAllOffers()
    {
        return shopOfferRepository.GetAll();
    }

    public List<ShopOffer> GetActiveOffers()
    {
        return shopOfferRepository.GetActiveOffers();
    }

    public List<ShopOffer> GetOffersByItemId(int itemId)
    {
        if (itemId <= 0)
            return new List<ShopOffer>();

        return shopOfferRepository.GetOffersByItemId(itemId);
    }

    public List<ShopOffer> GetOffersBySpellId(int spellId)
    {
        if (spellId <= 0)
            return new List<ShopOffer>();

        return shopOfferRepository.GetOffersBySpellId(spellId);
    }

    public int CreateOffer(ShopOffer shopOffer)
    {
        if (!IsValidOffer(shopOffer))
            return 0;

        return shopOfferRepository.Add(shopOffer);
    }

    public bool UpdateOffer(ShopOffer shopOffer)
    {
        if (shopOffer == null ||
            shopOffer.ShopOfferId <= 0 ||
            !IsValidOffer(shopOffer))
            return false;

        return shopOfferRepository.Update(shopOffer);
    }

    private bool IsValidOffer(ShopOffer shopOffer)
    {
        if (shopOffer == null)
            return false;

        if ((shopOffer.ItemId == null && shopOffer.SpellId == null) ||
            (shopOffer.ItemId != null && shopOffer.SpellId != null))
            return false;

        if (shopOffer.ItemId.HasValue && shopOffer.ItemId.Value <= 0)
            return false;

        if (shopOffer.SpellId.HasValue && shopOffer.SpellId.Value <= 0)
            return false;

        if (shopOffer.CoinPrice < 0 ||
            shopOffer.DiamondPrice < 0)
            return false;

        if (shopOffer.IsActive != 0 && shopOffer.IsActive != 1)
            return false;

        return true;
    }

    public bool DeleteOffer(int shopOfferId)
    {
        if (shopOfferId <= 0)
            return false;

        return shopOfferRepository.Delete(shopOfferId);
    }

    public bool OfferExists(int shopOfferId)
    {
        if (shopOfferId <= 0)
            return false;

        return shopOfferRepository.Exists(shopOfferId);
    }

    public int GetOfferCount()
    {
        return shopOfferRepository.Count();
    }

    public int GetActiveOfferCount()
    {
        return shopOfferRepository.CountActiveOffers();
    }
}