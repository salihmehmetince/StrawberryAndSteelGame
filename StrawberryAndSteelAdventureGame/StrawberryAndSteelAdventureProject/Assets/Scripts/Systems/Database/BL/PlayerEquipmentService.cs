
using System.Collections.Generic;

public class PlayerEquipmentService
{
    private readonly PlayerEquipmentRepository equipmentRepository;
    private readonly PlayerHasItemRepository playerHasItemRepository;
    private readonly ItemRepository itemRepository;

    public PlayerEquipmentService(
        PlayerEquipmentRepository equipmentRepository,
        PlayerHasItemRepository playerHasItemRepository,
        ItemRepository itemRepository)
    {
        this.equipmentRepository = equipmentRepository;
        this.playerHasItemRepository = playerHasItemRepository;
        this.itemRepository = itemRepository;
    }

    public PlayerEquipment GetById(int playerEquipmentId)
    {
        if (playerEquipmentId <= 0)
            return null;

        return equipmentRepository.GetById(playerEquipmentId);
    }

    public List<PlayerEquipment> GetPlayerEquipment(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerEquipment>();

        return equipmentRepository.GetByPlayerId(playerId);
    }

    public PlayerEquipment GetBySlot(int playerId, int equipmentSlotId)
    {
        if (playerId <= 0 || equipmentSlotId <= 0)
            return null;

        return equipmentRepository.GetByPlayerAndSlot(
            playerId,
            equipmentSlotId
        );
    }

    public bool EquipItem(int playerId, int itemId, int equipmentSlotId)
    {
        if (playerId <= 0 || itemId <= 0 || equipmentSlotId <= 0)
            return false;

        Item item = itemRepository.GetById(itemId);

        if (item == null ||
            !item.EquipmentSlotId.HasValue ||
            item.EquipmentSlotId.Value != equipmentSlotId)
        {
            return false;
        }

        PlayerHasItem playerItem =
            playerHasItemRepository.GetByPlayerAndItem(playerId, itemId);

        if (playerItem == null || playerItem.Quantity <= 0)
            return false;

        PlayerEquipment existingEquipment =
            equipmentRepository.GetByPlayerAndSlot(
                playerId,
                equipmentSlotId
            );

        if (existingEquipment != null)
        {
            existingEquipment.PlayerHasItemId = playerItem.PlayerHasItemId;

            return equipmentRepository.Update(existingEquipment);
        }

        PlayerEquipment equipment = new PlayerEquipment
        {
            PlayerId = playerId,
            PlayerHasItemId = playerItem.PlayerHasItemId,
            EquipmentSlotId = equipmentSlotId
        };

        return equipmentRepository.Add(equipment) > 0;
    }

    public bool UnequipItem(int playerId, int equipmentSlotId)
    {
        if (playerId <= 0 || equipmentSlotId <= 0)
            return false;

        PlayerEquipment equipment =
            equipmentRepository.GetByPlayerAndSlot(
                playerId,
                equipmentSlotId
            );

        if (equipment == null)
            return false;

        return equipmentRepository.Delete(equipment.PlayerEquipmentId);
    }

    public bool IsSlotOccupied(int playerId, int equipmentSlotId)
    {
        if (playerId <= 0 || equipmentSlotId <= 0)
            return false;

        return equipmentRepository.Exists(playerId, equipmentSlotId);
    }

    public int GetEquipmentCount(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return equipmentRepository.CountByPlayerId(playerId);
    }

    public int UnequipAll(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return equipmentRepository.DeleteByPlayerId(playerId);
    }

    public bool DeleteEquipment(int playerEquipmentId)
    {
        if (playerEquipmentId <= 0)
            return false;

        return equipmentRepository.Delete(playerEquipmentId);
    }
}
