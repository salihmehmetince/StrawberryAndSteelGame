using System.Collections.Generic;

public class ItemTypeService
{
    private readonly ItemTypeRepository itemTypeRepository;

    public ItemTypeService(ItemTypeRepository itemTypeRepository)
    {
        this.itemTypeRepository = itemTypeRepository;
    }

    public ItemType GetItemTypeById(int itemTypeId)
    {
        if (itemTypeId <= 0)
            return null;

        return itemTypeRepository.GetById(itemTypeId);
    }

    public ItemType GetItemTypeByName(string itemTypeName)
    {
        if (string.IsNullOrWhiteSpace(itemTypeName))
            return null;

        return itemTypeRepository.GetByName(itemTypeName.Trim());
    }

    public List<ItemType> GetAllItemTypes()
    {
        return itemTypeRepository.GetAll();
    }

    public int GetItemTypeCount()
    {
        return itemTypeRepository.Count();
    }
}