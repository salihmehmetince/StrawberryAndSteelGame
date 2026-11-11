using System.Collections.Generic;

public class EquipmentSlotService
{
    private readonly EquipmentSlotRepository equipmentSlotRepository;

    public EquipmentSlotService(
        EquipmentSlotRepository equipmentSlotRepository)
    {
        this.equipmentSlotRepository = equipmentSlotRepository;
    }

    public EquipmentSlot GetSlotById(int equipmentSlotId)
    {
        if (equipmentSlotId <= 0)
            return null;

        return equipmentSlotRepository.GetById(equipmentSlotId);
    }

    public EquipmentSlot GetSlotByName(string slotName)
    {
        if (string.IsNullOrWhiteSpace(slotName))
            return null;

        return equipmentSlotRepository.GetByName(slotName.Trim());
    }

    public List<EquipmentSlot> GetAllSlots()
    {
        return equipmentSlotRepository.GetAll();
    }

    public int CreateSlot(EquipmentSlot equipmentSlot)
    {
        if (equipmentSlot == null ||
            string.IsNullOrWhiteSpace(equipmentSlot.SlotName))
            return 0;

        equipmentSlot.SlotName = equipmentSlot.SlotName.Trim();

        if (equipmentSlotRepository.GetByName(equipmentSlot.SlotName) != null)
            return 0;

        return equipmentSlotRepository.Add(equipmentSlot);
    }

    public bool UpdateSlot(EquipmentSlot equipmentSlot)
    {
        if (equipmentSlot == null ||
            equipmentSlot.EquipmentSlotId <= 0 ||
            string.IsNullOrWhiteSpace(equipmentSlot.SlotName))
            return false;

        equipmentSlot.SlotName = equipmentSlot.SlotName.Trim();

        EquipmentSlot existingSlot =
            equipmentSlotRepository.GetByName(equipmentSlot.SlotName);

        if (existingSlot != null &&
            existingSlot.EquipmentSlotId != equipmentSlot.EquipmentSlotId)
            return false;

        return equipmentSlotRepository.Update(equipmentSlot);
    }

    public bool DeleteSlot(int equipmentSlotId)
    {
        if (equipmentSlotId <= 0)
            return false;

        return equipmentSlotRepository.Delete(equipmentSlotId);
    }

    public bool SlotExists(int equipmentSlotId)
    {
        if (equipmentSlotId <= 0)
            return false;

        return equipmentSlotRepository.Exists(equipmentSlotId);
    }

    public int GetSlotCount()
    {
        return equipmentSlotRepository.Count();
    }
}