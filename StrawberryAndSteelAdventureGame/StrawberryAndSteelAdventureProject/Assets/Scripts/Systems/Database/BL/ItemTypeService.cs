
using System;
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

    public int CreateItemType(ItemType itemType)
    {
        if (itemType == null)
            throw new ArgumentNullException(nameof(itemType));

        if (string.IsNullOrWhiteSpace(itemType.ItemTypeName))
            throw new ArgumentException("Item type name cannot be empty.");

        itemType.ItemTypeName = itemType.ItemTypeName.Trim();

        return itemTypeRepository.Add(itemType);
    }

    public bool UpdateItemType(ItemType itemType)
    {
        if (itemType == null || itemType.ItemTypeId <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(itemType.ItemTypeName))
            throw new ArgumentException("Item type name cannot be empty.");

        itemType.ItemTypeName = itemType.ItemTypeName.Trim();

        return itemTypeRepository.Update(itemType);
    }

    public bool DeleteItemType(int itemTypeId)
    {
        if (itemTypeId <= 0)
            return false;

        return itemTypeRepository.Delete(itemTypeId);
    }

    public bool ItemTypeExists(int itemTypeId)
    {
        if (itemTypeId <= 0)
            return false;

        return itemTypeRepository.Exists(itemTypeId);
    }

    public int GetItemTypeCount()
    {
        return itemTypeRepository.Count();
    }
}
