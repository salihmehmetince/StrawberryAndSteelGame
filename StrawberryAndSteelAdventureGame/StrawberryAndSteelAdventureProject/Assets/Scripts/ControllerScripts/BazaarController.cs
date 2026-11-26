
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BazaarController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private BazaarPanelController bazaarPanelController;
    [SerializeField] private GameObject bazaarPanel;
    [SerializeField] private GameObject bazaarPanelCloseButton;
    [SerializeField] private TMP_Text refreshCountdownText;

    [Header("Market Settings")]
    [SerializeField] private int refreshDurationMinutes = 30;

    [Tooltip("ItemTypeId values excluded from the bazaar.")]
    [SerializeField] private int[] excludedItemTypeIds;

    private int coinItemId;
    private int diamondItemId;

    private class BazaarCandidate
    {
        public Item Item;
        public ShopOffer Offer;
    }

    private PlayerMarket currentMarket;

    public event Action<int> OnBuyRequested;

    private GameBootstrapper Bootstrapper =>
        GameBootstrapper.Instance;

    private void Awake()
    {
        Debug.Log("[Bazaar] Awake started.");

        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OpenBazaar);
            Debug.Log("[Bazaar] Open button listener assigned.");
        }
        else
        {
            Debug.LogWarning("[Bazaar] Button component not found on this GameObject.");
        }

        if (bazaarPanelController != null)
        {
            bazaarPanelController.OnBuyRequested += HandleBuyRequested;
            Debug.Log("[Bazaar] BazaarPanelController event connected.");
        }
        else
        {
            Debug.LogError("[Bazaar] BazaarPanelController reference is NULL!");
        }

        Debug.Log("[Bazaar] Awake completed.");
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();

        if (button != null)
            button.onClick.RemoveListener(OpenBazaar);

        if (bazaarPanelController != null)
            bazaarPanelController.OnBuyRequested -= HandleBuyRequested;
    }

    private void Update()
    {
        UpdateCountdown();
    }

    private void LoadCurrencyItemIds()
    {
        Debug.Log("[Bazaar] Loading currency Item IDs.");

        List<Item> items = Bootstrapper.ItemService.GetAllItems();

        if (items == null)
            throw new InvalidOperationException("Items list is NULL.");

        Debug.Log("[Bazaar] Total items in database: " + items.Count);

        Item coin = items.FirstOrDefault(item => item.ItemName == "Coin");
        Item diamond = items.FirstOrDefault(item => item.ItemName == "Diamond");

        if (coin == null)
            throw new InvalidOperationException("Coin item not found.");

        if (diamond == null)
            throw new InvalidOperationException("Diamond item not found.");

        coinItemId = coin.ItemId;
        diamondItemId = diamond.ItemId;

        Debug.Log(
            "[Bazaar] Currency IDs loaded. Coin ID: " +
            coinItemId +
            ", Diamond ID: " +
            diamondItemId
        );
    }

    public void OpenBazaar()
    {
        Debug.Log("[Bazaar] OpenBazaar started.");

        int playerId = Bootstrapper.GameSession.CurrentPlayerId;

        Debug.Log("[Bazaar] Current Player ID: " + playerId);

        if (playerId <= 0)
        {
            Debug.LogError("[Bazaar] Valid player not found.");
            return;
        }

        try
        {
            LoadCurrencyItemIds();

            currentMarket =
                Bootstrapper.PlayerMarketService.GetMarketByPlayerId(playerId);

            if (currentMarket == null)
            {
                Debug.Log("[Bazaar] No existing market. Creating first market.");
                CreateFirstMarket(playerId);
            }
            else
            {
                Debug.Log(
                    "[Bazaar] Existing market found. Market ID: " +
                    currentMarket.PlayerMarketId +
                    ", Next refresh: " +
                    currentMarket.NextRefreshAtUtc
                );

                if (IsRefreshRequired(currentMarket))
                {
                    Debug.Log("[Bazaar] Market refresh required.");
                    RefreshMarket();
                }
                else
                {
                    Debug.Log("[Bazaar] Market refresh not required.");
                }
            }

            LoadSlots();

            if (bazaarPanel != null)
            {
                bazaarPanel.SetActive(true);
                Debug.Log("[Bazaar] Bazaar panel activated.");
            }
            else
            {
                Debug.LogError("[Bazaar] Bazaar panel reference is NULL!");
            }

            if (bazaarPanelCloseButton != null)
            {
                bazaarPanelCloseButton.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[Bazaar] Close button reference is NULL.");
            }

            Debug.Log("[Bazaar] OpenBazaar completed.");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[Bazaar] Error while opening bazaar: " +
                exception
            );
        }
    }

    private void CreateFirstMarket(int playerId)
    {
        Debug.Log("[Bazaar] CreateFirstMarket started.");

        List<BazaarCandidate> candidates = GetEligibleCandidates();

        Debug.Log(
            "[Bazaar] Eligible candidates: " +
            candidates.Count
        );

        if (candidates.Count == 0)
            throw new InvalidOperationException(
                "No eligible item offers found for the bazaar."
            );

        DateTime now = DateTime.UtcNow;

        PlayerMarket market = new PlayerMarket
        {
            PlayerId = playerId,
            CreatedAtUtc = now.ToString("o"),
            NextRefreshAtUtc = now.AddMinutes(refreshDurationMinutes).ToString("o")
        };

        Bootstrapper.DatabaseContext.Connection.RunInTransaction(() =>
        {
            int marketId =
                Bootstrapper.PlayerMarketService.CreateMarket(market);

            Debug.Log("[Bazaar] Created market ID: " + marketId);

            if (marketId <= 0)
                throw new InvalidOperationException(
                    "Player market could not be created."
                );

            CreateSixOffers(marketId, candidates);
        });

        currentMarket =
            Bootstrapper.PlayerMarketService.GetMarketByPlayerId(playerId);

        if (currentMarket == null)
            Debug.LogError("[Bazaar] Market could not be retrieved after creation.");
        else
            Debug.Log("[Bazaar] First market successfully loaded. ID: " + currentMarket.PlayerMarketId);
    }

    private void RefreshMarket()
    {
        Debug.Log("[Bazaar] RefreshMarket started.");

        List<BazaarCandidate> candidates = GetEligibleCandidates();

        Debug.Log("[Bazaar] Refresh candidates: " + candidates.Count);

        if (candidates.Count == 0)
            throw new InvalidOperationException(
                "No eligible item offers found for market refresh."
            );

        DateTime now = DateTime.UtcNow;

        string nextRefresh = now
            .AddMinutes(refreshDurationMinutes)
            .ToString("o");

        Bootstrapper.DatabaseContext.Connection.RunInTransaction(() =>
        {
            int deletedCount =
                Bootstrapper.PlayerMarketOfferService
                    .DeleteOffersByMarketId(currentMarket.PlayerMarketId);

            Debug.Log("[Bazaar] Deleted old offers. Result: " + deletedCount);

            CreateSixOffers(currentMarket.PlayerMarketId, candidates);

            currentMarket.NextRefreshAtUtc = nextRefresh;

            if (!Bootstrapper.PlayerMarketService.UpdateMarket(currentMarket))
                throw new InvalidOperationException(
                    "Market refresh time could not be updated."
                );
        });

        Debug.Log("[Bazaar] Market refresh completed.");
    }

    private void CreateSixOffers(
        int marketId,
        List<BazaarCandidate> candidates)
    {
        Debug.Log(
            "[Bazaar] Creating six offers. Market ID: " +
            marketId +
            ", Candidate count: " +
            candidates.Count
        );

        for (int slot = 1; slot <= 6; slot++)
        {
            BazaarCandidate candidate =
                candidates[UnityEngine.Random.Range(0, candidates.Count)];

            Debug.Log(
                "[Bazaar] Creating slot " + slot +
                " | Item ID: " + candidate.Item.ItemId +
                " | Item Name: " + candidate.Item.ItemName +
                " | Coin: " + candidate.Offer.CoinPrice +
                " | Diamond: " + candidate.Offer.DiamondPrice
            );

            PlayerMarketOffer offer = new PlayerMarketOffer
            {
                PlayerMarketId = marketId,
                SlotNumber = slot,
                ItemId = candidate.Item.ItemId,
                CoinPrice = candidate.Offer.CoinPrice,
                DiamondPrice = candidate.Offer.DiamondPrice,
                IsPurchased = 0,
                PurchasedAtUtc = null
            };

            int offerId =
                Bootstrapper.PlayerMarketOfferService.CreateOffer(offer);

            Debug.Log(
                "[Bazaar] Slot " + slot +
                " created. PlayerMarketOffer ID: " + offerId
            );

            if (offerId <= 0)
                throw new InvalidOperationException(
                    "Market offer could not be created for slot " + slot
                );
        }

        Debug.Log("[Bazaar] Six offers creation completed.");
    }

    private List<BazaarCandidate> GetEligibleCandidates()
    {
        Debug.Log("[Bazaar] GetEligibleCandidates started.");

        List<BazaarCandidate> candidates =
            new List<BazaarCandidate>();

        List<ShopOffer> activeOffers =
            Bootstrapper.ShopOfferService.GetActiveOffers();

        if (activeOffers == null)
            throw new InvalidOperationException("Active ShopOffers list is NULL.");

        Debug.Log(
            "[Bazaar] Active ShopOffer count: " +
            activeOffers.Count
        );

        foreach (ShopOffer shopOffer in activeOffers)
        {
            Debug.Log(
                "[Bazaar] Checking ShopOffer ID: " +
                shopOffer.ShopOfferId +
                " | ItemId: " +
                shopOffer.ItemId +
                " | Active: " +
                shopOffer.IsActive
            );

            if (!shopOffer.ItemId.HasValue)
            {
                Debug.Log("[Bazaar] Skipped: ShopOffer has no ItemId.");
                continue;
            }

            Item item = Bootstrapper.ItemService.GetItemById(
                shopOffer.ItemId.Value
            );

            if (item == null)
            {
                Debug.LogWarning(
                    "[Bazaar] Skipped: Item not found. Item ID: " +
                    shopOffer.ItemId.Value
                );
                continue;
            }

            Debug.Log(
                "[Bazaar] Item found: " +
                item.ItemName +
                " | Item ID: " +
                item.ItemId +
                " | Type ID: " +
                item.ItemTypeId +
                " | Active: " +
                item.IsActive
            );

            if (item.IsActive != 1)
            {
                Debug.Log("[Bazaar] Skipped: Item is inactive.");
                continue;
            }

            if (excludedItemTypeIds != null &&
                excludedItemTypeIds.Contains(item.ItemTypeId))
            {
                Debug.Log(
                    "[Bazaar] Skipped: ItemTypeId excluded. Type ID: " +
                    item.ItemTypeId
                );
                continue;
            }

            candidates.Add(new BazaarCandidate
            {
                Item = item,
                Offer = shopOffer
            });

            Debug.Log(
                "[Bazaar] Candidate accepted: " +
                item.ItemName
            );
        }

        Debug.Log(
            "[Bazaar] GetEligibleCandidates completed. Total: " +
            candidates.Count
        );

        return candidates;
    }

    private bool IsRefreshRequired(PlayerMarket market)
    {
        if (!DateTime.TryParse(
            market.NextRefreshAtUtc,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out DateTime refreshTime))
        {
            Debug.LogWarning("[Bazaar] Invalid refresh date. Refresh required.");
            return true;
        }

        bool required = DateTime.UtcNow >= refreshTime.ToUniversalTime();

        Debug.Log(
            "[Bazaar] Refresh check: " +
            required +
            " | Current UTC: " +
            DateTime.UtcNow.ToString("o") +
            " | Refresh UTC: " +
            refreshTime.ToUniversalTime().ToString("o")
        );

        return required;
    }

    private void LoadSlots()
    {
        Debug.Log("[Bazaar] LoadSlots started.");

        if (currentMarket == null)
        {
            Debug.LogError("[Bazaar] Cannot load slots. Current market is NULL.");
            return;
        }

        List<PlayerMarketOffer> offers =
            Bootstrapper.PlayerMarketOfferService
                .GetOffersByMarketId(currentMarket.PlayerMarketId);

        if (offers == null)
            throw new InvalidOperationException("PlayerMarketOffer list is NULL.");

        Debug.Log(
            "[Bazaar] Offers loaded from database: " +
            offers.Count +
            " | Market ID: " +
            currentMarket.PlayerMarketId
        );

        BazaarPanelController.BazaarSlotData[] slotData =
            new BazaarPanelController.BazaarSlotData[6];

        for (int i = 0; i < slotData.Length; i++)
        {
            int slotNumber = i + 1;

            PlayerMarketOffer offer =
                offers.FirstOrDefault(x => x.SlotNumber == slotNumber);

            if (offer == null)
            {
                Debug.LogWarning(
                    "[Bazaar] Slot " + slotNumber +
                    " has NO PlayerMarketOffer record."
                );

                slotData[i] = new BazaarPanelController.BazaarSlotData
                {
                    IsPurchased = true
                };

                continue;
            }

            Debug.Log(
                "[Bazaar] Slot " + slotNumber +
                " offer found | Offer ID: " + offer.PlayerMarketOfferId +
                " | Item ID: " + offer.ItemId +
                " | Purchased: " + offer.IsPurchased +
                " | Coin: " + offer.CoinPrice +
                " | Diamond: " + offer.DiamondPrice
            );

            if (offer.IsPurchased == 1)
            {
                Debug.Log("[Bazaar] Slot " + slotNumber + " is already purchased.");

                slotData[i] = new BazaarPanelController.BazaarSlotData
                {
                    IsPurchased = true
                };

                continue;
            }

            Item item =
                Bootstrapper.ItemService.GetItemById(offer.ItemId);

            if (item == null)
            {
                Debug.LogError(
                    "[Bazaar] Slot " + slotNumber +
                    " item not found. Item ID: " + offer.ItemId
                );

                slotData[i] = new BazaarPanelController.BazaarSlotData
                {
                    IsPurchased = true
                };

                continue;
            }

            Sprite itemSprite = Resources.Load<Sprite>(item.ItemImage);

            if (itemSprite == null)
            {
                Debug.LogError(
                    "[Bazaar] Sprite not found in Resources. Item: " +
                    item.ItemName +
                    " | Resource path: " +
                    item.ItemImage
                );
            }
            else
            {
                Debug.Log(
                    "[Bazaar] Sprite loaded: " +
                    item.ItemName +
                    " | " +
                    item.ItemImage
                );
            }

            slotData[i] = new BazaarPanelController.BazaarSlotData
            {
                ItemImage = itemSprite,
                ItemName = item.ItemName,
                Damage = 0,
                CoinPrice = offer.CoinPrice,
                DiamondPrice = offer.DiamondPrice,
                IsPurchased = false
            };

            Debug.Log(
                "[Bazaar] Slot data prepared: " +
                slotNumber +
                " | Item: " + slotData[i].ItemName +
                " | Coin: " + slotData[i].CoinPrice +
                " | Diamond: " + slotData[i].DiamondPrice +
                " | Purchased: " + slotData[i].IsPurchased
            );
        }

        Debug.Log("[Bazaar] Sending slot data to BazaarPanelController.");

        if (bazaarPanelController == null)
        {
            Debug.LogError("[Bazaar] Cannot display slots. Panel controller is NULL.");
            return;
        }

        bazaarPanelController.DisplaySlots(slotData);

        Debug.Log("[Bazaar] LoadSlots completed.");
    }

    private void HandleBuyRequested(int slotNumber)
    {
        Debug.Log("[Bazaar] Buy requested. Slot: " + slotNumber);

        PurchaseOffer(slotNumber);

        OnBuyRequested?.Invoke(slotNumber);
    }

    private void PurchaseOffer(int slotNumber)
    {
        Debug.Log("[Bazaar] PurchaseOffer started. Slot: " + slotNumber);

        int playerId = Bootstrapper.GameSession.CurrentPlayerId;

        if (playerId <= 0)
        {
            Debug.LogError("[Bazaar] Valid player not found.");
            return;
        }

        if (currentMarket == null)
        {
            Debug.LogError("[Bazaar] Player market not found.");
            return;
        }

        if (slotNumber < 1 || slotNumber > 6)
        {
            Debug.LogError("[Bazaar] Invalid market slot.");
            return;
        }

        if (coinItemId <= 0 || diamondItemId <= 0)
        {
            Debug.LogError("[Bazaar] Currency Item IDs are not loaded.");
            return;
        }

        try
        {
            Bootstrapper.DatabaseContext.Connection.RunInTransaction(() =>
            {
                PlayerMarketOffer offer =
                    Bootstrapper.PlayerMarketOfferService.GetOfferBySlot(
                        currentMarket.PlayerMarketId,
                        slotNumber
                    );

                if (offer == null)
                    throw new InvalidOperationException("No offer found in this slot.");

                if (offer.IsPurchased == 1)
                    throw new InvalidOperationException("This offer has already been purchased.");

                if (offer.CoinPrice < 0 || offer.DiamondPrice < 0)
                    throw new InvalidOperationException("Invalid offer price.");

                int playerCoin =
                    Bootstrapper.PlayerHasItemService.GetItemQuantity(
                        playerId,
                        coinItemId
                    );

                int playerDiamond =
                    Bootstrapper.PlayerHasItemService.GetItemQuantity(
                        playerId,
                        diamondItemId
                    );

                Debug.Log(
                    "[Bazaar] Purchase balances | Coin: " + playerCoin +
                    " | Diamond: " + playerDiamond
                );

                if (playerCoin < offer.CoinPrice)
                    throw new InvalidOperationException("Not enough Coins.");

                if (playerDiamond < offer.DiamondPrice)
                    throw new InvalidOperationException("Not enough Diamonds.");

                if (offer.CoinPrice > 0 &&
                    !Bootstrapper.PlayerHasItemService.RemoveQuantity(
                        playerId, coinItemId, offer.CoinPrice))
                    throw new InvalidOperationException("Coin deduction failed.");

                if (offer.DiamondPrice > 0 &&
                    !Bootstrapper.PlayerHasItemService.RemoveQuantity(
                        playerId, diamondItemId, offer.DiamondPrice))
                    throw new InvalidOperationException("Diamond deduction failed.");

                int addedItemId =
                    Bootstrapper.PlayerHasItemService.AddItem(
                        playerId,
                        offer.ItemId,
                        1
                    );

                if (addedItemId <= 0)
                    throw new InvalidOperationException("Purchased item could not be added to inventory.");

                bool markedAsPurchased =
                    Bootstrapper.PlayerMarketOfferService.MarkAsPurchased(
                        offer.PlayerMarketOfferId,
                        DateTime.UtcNow.ToString("o")
                    );

                if (!markedAsPurchased)
                    throw new InvalidOperationException("Offer could not be marked as purchased.");
            });

            Debug.Log("[Bazaar] Purchase successful. Slot: " + slotNumber);

            LoadSlots();
        }
        catch (Exception exception)
        {
            Debug.LogError("[Bazaar] Purchase failed: " + exception);
        }
    }

    private void UpdateCountdown()
    {
        if (refreshCountdownText == null || currentMarket == null)
            return;

        if (!DateTime.TryParse(
            currentMarket.NextRefreshAtUtc,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out DateTime refreshTime))
        {
            refreshCountdownText.text = "Refresh pending";
            return;
        }

        TimeSpan remaining =
            refreshTime.ToUniversalTime() - DateTime.UtcNow;

        if (remaining.TotalSeconds <= 0)
        {
            refreshCountdownText.text = "Ready to refresh";
            return;
        }

        refreshCountdownText.text =
            $"{(int)remaining.TotalHours:00}:" +
            $"{remaining.Minutes:00}:" +
            $"{remaining.Seconds:00}";
    }
}
