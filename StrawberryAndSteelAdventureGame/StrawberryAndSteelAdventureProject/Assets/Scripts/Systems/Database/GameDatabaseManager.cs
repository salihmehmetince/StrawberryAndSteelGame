
using System.Collections.Generic;
using SQLite;
using UnityEngine;

public class GameDatabaseManager : MonoBehaviour
{
    private SQLiteConnection Database
    {
        get
        {
            if (GameDatabase.Instance == null)
                throw new System.Exception("GameDatabase is not initialized.");

            return GameDatabase.Instance.Database;
        }
    }

    // =====================================================
    // PLAYER
    // =====================================================

    public int CreatePlayer(PlayerData player)
    {
        Database.Insert(player);
        return player.PlayerId;
    }

    public PlayerData GetPlayer(int playerId)
    {
        return Database.Find<PlayerData>(playerId);
    }

    public List<PlayerData> GetAllPlayers()
    {
        return Database.Query<PlayerData>(
            "SELECT * FROM Player ORDER BY PlayerId LIMIT 3"
        );
    }

    public bool UpdatePlayer(PlayerData player)
    {
        return Database.Update(player) > 0;
    }

    public bool DeletePlayer(int playerId)
    {
        return Database.Delete<PlayerData>(playerId) > 0;
    }

    // =====================================================
    // ITEM TYPES
    // =====================================================

    public int CreateItemType(ItemTypeData itemType)
    {
        Database.Insert(itemType);
        return itemType.ItemTypeId;
    }

    public ItemTypeData GetItemType(int itemTypeId)
    {
        return Database.Find<ItemTypeData>(itemTypeId);
    }

    public List<ItemTypeData> GetAllItemTypes()
    {
        return Database.Table<ItemTypeData>().ToList();
    }

    public bool UpdateItemType(ItemTypeData itemType)
    {
        return Database.Update(itemType) > 0;
    }

    public bool DeleteItemType(int itemTypeId)
    {
        return Database.Delete<ItemTypeData>(itemTypeId) > 0;
    }

    // =====================================================
    // ITEM
    // =====================================================

    public int CreateItem(ItemData item)
    {
        Database.Insert(item);
        return item.ItemId;
    }

    public ItemData GetItem(int itemId)
    {
        return Database.Find<ItemData>(itemId);
    }

    public List<ItemData> GetAllItems()
    {
        return Database.Table<ItemData>().ToList();
    }

    public List<ItemData> GetItemsByType(int itemTypeId)
    {
        return Database.Query<ItemData>(
            "SELECT * FROM Item WHERE ItemTypeId = ?",
            itemTypeId
        );
    }

    public bool UpdateItem(ItemData item)
    {
        return Database.Update(item) > 0;
    }

    public bool DeleteItem(int itemId)
    {
        return Database.Delete<ItemData>(itemId) > 0;
    }

    // =====================================================
    // PLAYER HAS ITEM
    // =====================================================

    public int AddPlayerItem(PlayerHasItemData playerItem)
    {
        Database.Insert(playerItem);
        return playerItem.PlayerHasItemId;
    }

    public PlayerHasItemData GetPlayerItem(int playerHasItemId)
    {
        return Database.Find<PlayerHasItemData>(playerHasItemId);
    }

    public List<PlayerHasItemData> GetPlayerInventory(int playerId)
    {
        return Database.Query<PlayerHasItemData>(
            "SELECT * FROM PlayerHasItem WHERE PlayerId = ?",
            playerId
        );
    }

    public bool UpdatePlayerItem(PlayerHasItemData playerItem)
    {
        return Database.Update(playerItem) > 0;
    }

    public bool RemovePlayerItem(int playerHasItemId)
    {
        return Database.Delete<PlayerHasItemData>(playerHasItemId) > 0;
    }

    // =====================================================
    // PLAYER PROGRESS
    // =====================================================

    public int CreatePlayerProgress(PlayerProgressData progress)
    {
        Database.Insert(progress);
        return progress.PlayerProgressId;
    }

    public PlayerProgressData GetPlayerProgress(int playerId)
    {
        return Database.Find<PlayerProgressData>(
            p => p.PlayerId == playerId
        );
    }

    public bool UpdatePlayerProgress(PlayerProgressData progress)
    {
        return Database.Update(progress) > 0;
    }

    public bool DeletePlayerProgress(int playerProgressId)
    {
        return Database.Delete<PlayerProgressData>(playerProgressId) > 0;
    }

    // =====================================================
    // PLAYER SETTINGS
    // =====================================================

    public void CreatePlayerSettings(PlayerSettingsData settings)
    {
        Database.Insert(settings);
    }

    public PlayerSettingsData GetPlayerSettings(int playerId)
    {
        return Database.Find<PlayerSettingsData>(playerId);
    }

    public bool UpdatePlayerSettings(PlayerSettingsData settings)
    {
        return Database.Update(settings) > 0;
    }

    public bool DeletePlayerSettings(int playerId)
    {
        return Database.Delete<PlayerSettingsData>(playerId) > 0;
    }

    // =====================================================
    // PLAYER EQUIPMENT
    // =====================================================

    public int EquipItem(PlayerEquipmentData equipment)
    {
        Database.Insert(equipment);
        return equipment.PlayerEquipmentId;
    }

    public List<PlayerEquipmentData> GetPlayerEquipment(int playerId)
    {
        return Database.Query<PlayerEquipmentData>(
            "SELECT * FROM PlayerEquipment WHERE PlayerId = ?",
            playerId
        );
    }

    public bool UpdateEquipment(PlayerEquipmentData equipment)
    {
        return Database.Update(equipment) > 0;
    }

    public bool UnequipItem(int playerEquipmentId)
    {
        return Database.Delete<PlayerEquipmentData>(
            playerEquipmentId
        ) > 0;
    }

    public bool DeleteAllPlayerEquipment(int playerId)
    {
        return Database.Execute(
            "DELETE FROM PlayerEquipment WHERE PlayerId = ?",
            playerId
        ) > 0;
    }


    public PlayerData CreatePlayerWithDefaults(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            throw new System.ArgumentException("Player name cannot be empty.");

        if (GetAllPlayers().Count >= 3)
            throw new System.InvalidOperationException("All save slots are full.");

        PlayerData player = new PlayerData
        {
            PlayerName = playerName.Trim(),
            CreatedAt = System.DateTime.Now.ToString("o")
        };

        Database.RunInTransaction(() =>
        {
            Database.Insert(player);

            Database.Insert(new PlayerProgressData
            {
                PlayerId = player.PlayerId,
                LastUnlockedLevel = 1,
                CurrentLevel = 1
            });

            Database.Insert(new PlayerSettingsData
            {
                PlayerId = player.PlayerId,
                AudioLevel = 1f,
                MusicLevel = 1f,
                Sensitivity = 1f
            });
        });

        return player;
    }

    public PlayerData OverwritePlayerWithDefaults(int playerId, string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            throw new System.ArgumentException("Player name cannot be empty.");

        PlayerData player = GetPlayer(playerId);

        if (player == null)
            throw new System.InvalidOperationException("Player record not found.");

        Database.RunInTransaction(() =>
        {
            // Remove old equipment.
            Database.Execute(
                "DELETE FROM PlayerEquipment WHERE PlayerId = ?",
                playerId
            );

            // Remove old inventory.
            Database.Execute(
                "DELETE FROM PlayerHasItem WHERE PlayerId = ?",
                playerId
            );

            // Remove old settings and progress.
            Database.Execute(
                "DELETE FROM PlayerSettings WHERE PlayerId = ?",
                playerId
            );

            Database.Execute(
                "DELETE FROM PlayerProgress WHERE PlayerId = ?",
                playerId
            );

            // Update the existing player record.
            player.PlayerName = playerName.Trim();
            player.CreatedAt = System.DateTime.Now.ToString("o");

            Database.Update(player);

            // Create default progress.
            Database.Insert(new PlayerProgressData
            {
                PlayerId = playerId,
                LastUnlockedLevel = 1,
                CurrentLevel = 1
            });

            // Create default settings.
            Database.Insert(new PlayerSettingsData
            {
                PlayerId = playerId,
                AudioLevel = 1f,
                MusicLevel = 1f,
                Sensitivity = 1f
            });
        });

        return GetPlayer(playerId);
    }

}

// =========================================================
// DATABASE MODELS
// =========================================================

[Table("Player")]
public class PlayerData
{
    [PrimaryKey, AutoIncrement]
    public int PlayerId { get; set; }

    [NotNull]
    public string PlayerName { get; set; }

    [NotNull]
    public string CreatedAt { get; set; }
}

[Table("ItemTypes")]
public class ItemTypeData
{
    [PrimaryKey]
    public int ItemTypeId { get; set; }

    [NotNull, Unique]
    public string ItemTypeName { get; set; }
}

[Table("Item")]
public class ItemData
{
    [PrimaryKey]
    public int ItemId { get; set; }

    [NotNull]
    public string ItemName { get; set; }

    public int ItemTypeId { get; set; }

    public string ItemDescription { get; set; }

    public int ItemValue { get; set; }

    public int IsStackable { get; set; }

    public int MaxStack { get; set; }
}

[Table("PlayerHasItem")]
public class PlayerHasItemData
{
    [PrimaryKey, AutoIncrement]
    public int PlayerHasItemId { get; set; }

    public int PlayerId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }
}

[Table("PlayerProgress")]
public class PlayerProgressData
{
    [PrimaryKey, AutoIncrement]
    public int PlayerProgressId { get; set; }

    [Unique]
    public int PlayerId { get; set; }

    public int LastUnlockedLevel { get; set; }

    public int CurrentLevel { get; set; }
}

[Table("PlayerSettings")]
public class PlayerSettingsData
{
    [PrimaryKey]
    public int PlayerId { get; set; }

    public float AudioLevel { get; set; }

    public float MusicLevel { get; set; }

    public float Sensitivity { get; set; }
}

[Table("PlayerEquipment")]
public class PlayerEquipmentData
{
    [PrimaryKey, AutoIncrement]
    public int PlayerEquipmentId { get; set; }

    public int PlayerId { get; set; }

    public int PlayerHasItemId { get; set; }
}
