
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectionSceneCollectables : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text diamondText;

    private void Start()
    {
        RefreshCounts();
    }

    public void RefreshCounts()
    {
        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper == null ||
            !bootstrapper.GameSession.HasActivePlayer)
        {
            Debug.LogError("Active player not found.");
            return;
        }

        int playerId = bootstrapper.GameSession.CurrentPlayerId;

        ItemType itemType =
            bootstrapper.ItemTypeService.GetItemTypeByName("Collectable");

        if (itemType == null)
        {
            Debug.LogError("Collectable item type not found.");
            return;
        }

        List<Item> items =
            bootstrapper.ItemService.GetItemsByTypeId(itemType.ItemTypeId);

        int coinItemId = 0;
        int diamondItemId = 0;

        foreach (Item item in items)
        {
            if (item.ItemName == "Coin")
                coinItemId = item.ItemId;
            else if (item.ItemName == "Diamond")
                diamondItemId = item.ItemId;
        }

        List<PlayerHasItem> inventory =
            bootstrapper.PlayerHasItemService.GetInventory(playerId);

        int totalCoins = 0;
        int totalDiamonds = 0;

        foreach (PlayerHasItem inventoryItem in inventory)
        {
            if (inventoryItem.ItemId == coinItemId)
                totalCoins = inventoryItem.Quantity;

            else if (inventoryItem.ItemId == diamondItemId)
                totalDiamonds = inventoryItem.Quantity;
        }

        if (coinText != null)
            coinText.text = totalCoins.ToString();

        if (diamondText != null)
            diamondText.text = totalDiamonds.ToString();
    }
}
