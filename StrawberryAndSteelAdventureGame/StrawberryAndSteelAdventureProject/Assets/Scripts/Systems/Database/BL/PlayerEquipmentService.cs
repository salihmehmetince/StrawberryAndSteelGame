using System.Collections.Generic;

public class PlayerEquipmentService
{
    private readonly PlayerEquipmentRepository equipmentRepository;
    private readonly PlayerHasItemRepository playerHasItemRepository;

    public PlayerEquipmentService(
        PlayerEquipmentRepository equipmentRepository,
        PlayerHasItemRepository playerHasItemRepository)
    {
        this.equipmentRepository = equipmentRepository;
        this.playerHasItemRepository = playerHasItemRepository;
    }

    public List<PlayerEquipment> GetEquippedItems(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerEquipment>();

        return equipmentRepository.GetByPlayerId(playerId);
    }

    public bool EquipItem(int playerId, int playerHasItemId)
    {
        if (playerId <= 0 || playerHasItemId <= 0)
            return false;

        PlayerHasItem inventoryItem =
            playerHasItemRepository.GetById(playerHasItemId);

        if (inventoryItem == null || inventoryItem.PlayerId != playerId)
            return false;

        if (equipmentRepository.Exists(playerId, playerHasItemId))
            return false;

        PlayerEquipment equipment = new PlayerEquipment
        {
            PlayerId = playerId,
            PlayerHasItemId = playerHasItemId
        };

        return equipmentRepository.Add(equipment) > 0;
    }

    public bool UnequipItem(int playerEquipmentId)
    {
        if (playerEquipmentId <= 0)
            return false;

        return equipmentRepository.Delete(playerEquipmentId);
    }

    public bool IsEquipped(int playerId, int playerHasItemId)
    {
        if (playerId <= 0 || playerHasItemId <= 0)
            return false;

        return equipmentRepository.Exists(playerId, playerHasItemId);
    }

    public bool UnequipAll(int playerId)
    {
        if (playerId <= 0)
            return false;

        equipmentRepository.DeleteByPlayerId(playerId);
        return true;
    }
}