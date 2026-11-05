using System;
using System.Collections.Generic;

public class PlayerHasItemService
{
    private readonly PlayerHasItemRepository playerHasItemRepository;
    private readonly ItemRepository itemRepository;

    public PlayerHasItemService(
        PlayerHasItemRepository playerHasItemRepository,
        ItemRepository itemRepository)
    {
        this.playerHasItemRepository = playerHasItemRepository;
        this.itemRepository = itemRepository;
    }

    public List<PlayerHasItem> GetInventory(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerHasItem>();

        return playerHasItemRepository.GetByPlayerId(playerId);
    }

    public bool AddItem(int playerId, int itemId, int quantity)
    {
        if (playerId <= 0 || itemId <= 0 || quantity <= 0)
            return false;

        Item item = itemRepository.GetById(itemId);

        if (item == null)
            return false;

        PlayerHasItem existingItem =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        if (existingItem != null)
        {
            int newQuantity = existingItem.Quantity + quantity;

            if (item.IsStackable == 0 || newQuantity > item.MaxStack)
                return false;

            existingItem.Quantity = newQuantity;

            return playerHasItemRepository.Update(existingItem);
        }

        if (item.IsStackable == 0 && quantity > 1)
            return false;

        if (quantity > item.MaxStack)
            return false;

        PlayerHasItem playerHasItem = new PlayerHasItem
        {
            PlayerId = playerId,
            ItemId = itemId,
            Quantity = quantity
        };

        return playerHasItemRepository.Add(playerHasItem) > 0;
    }

    public bool RemoveItem(int playerId, int itemId, int quantity)
    {
        if (playerId <= 0 || itemId <= 0 || quantity <= 0)
            return false;

        PlayerHasItem existingItem =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        if (existingItem == null || existingItem.Quantity < quantity)
            return false;

        existingItem.Quantity -= quantity;

        if (existingItem.Quantity == 0)
        {
            return playerHasItemRepository.Delete(
                existingItem.PlayerHasItemId
            );
        }

        return playerHasItemRepository.Update(existingItem);
    }

    public bool HasItem(int playerId, int itemId)
    {
        if (playerId <= 0 || itemId <= 0)
            return false;

        return playerHasItemRepository.Exists(playerId, itemId);
    }

    public bool DeleteInventory(int playerId)
    {
        if (playerId <= 0)
            return false;

        playerHasItemRepository.DeleteByPlayerId(playerId);
        return true;
    }
}