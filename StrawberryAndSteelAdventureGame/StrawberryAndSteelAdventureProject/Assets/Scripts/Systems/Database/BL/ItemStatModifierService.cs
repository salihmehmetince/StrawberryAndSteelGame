using System.Collections.Generic;

public class ItemStatModifierService
{
    private readonly ItemStatModifierRepository modifierRepository;

    public ItemStatModifierService(ItemStatModifierRepository modifierRepository)
    {
        this.modifierRepository = modifierRepository;
    }

    public ItemStatModifier GetModifierById(int modifierId)
    {
        if (modifierId <= 0)
            return null;

        return modifierRepository.GetById(modifierId);
    }

    public List<ItemStatModifier> GetModifiersByItemId(int itemId)
    {
        if (itemId <= 0)
            return new List<ItemStatModifier>();

        return modifierRepository.GetByItemId(itemId);
    }

    public List<ItemStatModifier> GetAllModifiers()
    {
        return modifierRepository.GetAll();
    }

    public int CreateModifier(ItemStatModifier modifier)
    {
        if (modifier == null)
            return 0;

        if (modifier.ItemId <= 0 ||
            string.IsNullOrWhiteSpace(modifier.StatType) ||
            modifier.ModifierValue <= 0 ||
            float.IsNaN(modifier.ModifierValue) ||
            float.IsInfinity(modifier.ModifierValue))
            return 0;

        modifier.StatType = modifier.StatType.Trim();

        return modifierRepository.Add(modifier);
    }

    public bool UpdateModifier(ItemStatModifier modifier)
    {
        if (modifier == null)
            return false;

        if (modifier.ItemStatModifierId <= 0 ||
            modifier.ItemId <= 0 ||
            string.IsNullOrWhiteSpace(modifier.StatType) ||
            modifier.ModifierValue <= 0 ||
            float.IsNaN(modifier.ModifierValue) ||
            float.IsInfinity(modifier.ModifierValue))
            return false;

        modifier.StatType = modifier.StatType.Trim();

        return modifierRepository.Update(modifier);
    }

    public bool DeleteModifier(int modifierId)
    {
        if (modifierId <= 0)
            return false;

        return modifierRepository.Delete(modifierId);
    }

    public int DeleteModifiersByItemId(int itemId)
    {
        if (itemId <= 0)
            return 0;

        return modifierRepository.DeleteByItemId(itemId);
    }

    public bool ModifierExists(int modifierId)
    {
        if (modifierId <= 0)
            return false;

        return modifierRepository.Exists(modifierId);
    }

    public int GetModifierCount(int itemId)
    {
        if (itemId <= 0)
            return 0;

        return modifierRepository.CountByItemId(itemId);
    }
}