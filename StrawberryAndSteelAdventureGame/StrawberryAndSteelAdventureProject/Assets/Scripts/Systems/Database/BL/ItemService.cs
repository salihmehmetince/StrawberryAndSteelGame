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

    public List<Item> GetAllItems()
    {
        return itemRepository.GetAll();
    }

    public List<Item> GetItemsByTypeId(int itemTypeId)
    {
        if (itemTypeId <= 0)
            return new List<Item>();

        return itemRepository.GetByTypeId(itemTypeId);
    }

    public int GetItemCount()
    {
        return itemRepository.Count();
    }
}