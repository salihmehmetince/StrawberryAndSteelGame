
using System;
using System.Collections.Generic;

public class PlayerHasItemService
{
    private readonly PlayerHasItemRepository playerHasItemRepository;

    public PlayerHasItemService(
        PlayerHasItemRepository playerHasItemRepository)
    {
        this.playerHasItemRepository = playerHasItemRepository;
    }

    public PlayerHasItem GetById(int playerHasItemId)
    {
        if (playerHasItemId <= 0)
            return null;

        return playerHasItemRepository.GetById(playerHasItemId);
    }

    public PlayerHasItem GetByPlayerAndItem(int playerId, int itemId)
    {
        if (playerId <= 0 || itemId <= 0)
            return null;

        return playerHasItemRepository.GetByPlayerAndItem(
            playerId,
            itemId
        );
    }

    public List<PlayerHasItem> GetPlayerItems(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerHasItem>();

        return playerHasItemRepository.GetByPlayerId(playerId);
    }

    public int AddItem(int playerId, int itemId, int quantity)
    {
        if (playerId <= 0 || itemId <= 0 || quantity <= 0)
            return 0;

        PlayerHasItem existingItem =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        if (existingItem != null)
        {
            try
            {
                existingItem.Quantity = checked(
                    existingItem.Quantity + quantity
                );
            }
            catch (OverflowException)
            {
                return 0;
            }

            return playerHasItemRepository.Update(existingItem)
                ? existingItem.PlayerHasItemId
                : 0;
        }

        PlayerHasItem playerHasItem = new PlayerHasItem
        {
            PlayerId = playerId,
            ItemId = itemId,
            Quantity = quantity
        };

        return playerHasItemRepository.Add(playerHasItem);
    }

    public bool RemoveQuantity(int playerId, int itemId, int quantity)
    {
        if (playerId <= 0 || itemId <= 0 || quantity <= 0)
            return false;

        PlayerHasItem existingItem =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        if (existingItem == null || existingItem.Quantity < quantity)
            return false;

        if (existingItem.Quantity == quantity)
        {
            return playerHasItemRepository.Delete(
                existingItem.PlayerHasItemId
            );
        }

        existingItem.Quantity -= quantity;

        return playerHasItemRepository.Update(existingItem);
    }

    public bool SetQuantity(int playerId, int itemId, int quantity)
    {
        if (playerId <= 0 || itemId <= 0 || quantity < 0)
            return false;

        PlayerHasItem existingItem =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        if (existingItem == null)
            return false;

        if (quantity == 0)
        {
            return playerHasItemRepository.Delete(
                existingItem.PlayerHasItemId
            );
        }

        existingItem.Quantity = quantity;

        return playerHasItemRepository.Update(existingItem);
    }

    public bool HasItem(int playerId, int itemId)
    {
        if (playerId <= 0 || itemId <= 0)
            return false;

        return playerHasItemRepository.Exists(playerId, itemId);
    }

    public int GetItemQuantity(int playerId, int itemId)
    {
        if (playerId <= 0 || itemId <= 0)
            return 0;

        PlayerHasItem item =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        return item != null ? item.Quantity : 0;
    }

    public int GetItemCount(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return playerHasItemRepository.CountByPlayerId(playerId);
    }

    public bool DeleteItem(int playerHasItemId)
    {
        if (playerHasItemId <= 0)
            return false;

        return playerHasItemRepository.Delete(playerHasItemId);
    }

    public int DeletePlayerItems(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return playerHasItemRepository.DeleteByPlayerId(playerId);
    }
}
