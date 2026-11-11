
using System;
using System.Collections.Generic;

public class ItemService
{
    private readonly ItemRepository itemRepository;

    public ItemService(ItemRepository itemRepository)
    {
        this.itemRepository = itemRepository;
    }

    public Item GetItemById(int itemId)
    {
        if (itemId <= 0)
            return null;

        return itemRepository.GetById(itemId);
    }

    public List<Item> GetItemsByType(int itemTypeId)
    {
        if (itemTypeId <= 0)
            return new List<Item>();

        return itemRepository.GetByTypeId(itemTypeId);
    }

    public List<Item> GetItemsByEquipmentSlot(int equipmentSlotId)
    {
        if (equipmentSlotId <= 0)
            return new List<Item>();

        return itemRepository.GetByEquipmentSlotId(equipmentSlotId);
    }

    public List<Item> GetAllItems()
    {
        return itemRepository.GetAll();
    }

    public int CreateItem(Item item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (string.IsNullOrWhiteSpace(item.ItemName))
            throw new ArgumentException("Item name cannot be empty.");

        if (item.ItemTypeId <= 0)
            throw new ArgumentException("Invalid item type ID.");

        item.ItemName = item.ItemName.Trim();

        return itemRepository.Add(item);
    }

    public bool UpdateItem(Item item)
    {
        if (item == null || item.ItemId <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(item.ItemName))
            throw new ArgumentException("Item name cannot be empty.");

        if (item.ItemTypeId <= 0)
            return false;

        item.ItemName = item.ItemName.Trim();

        return itemRepository.Update(item);
    }

    public bool DeleteItem(int itemId)
    {
        if (itemId <= 0)
            return false;

        return itemRepository.Delete(itemId);
    }

    public bool ItemExists(int itemId)
    {
        if (itemId <= 0)
            return false;

        return itemRepository.Exists(itemId);
    }

    public int GetItemCount()
    {
        return itemRepository.Count();
    }
}
