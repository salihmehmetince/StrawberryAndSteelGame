
using System.IO;
using SQLite;
using UnityEngine;

public class GameDatabase : MonoBehaviour
{
    private static GameDatabase instance;

    private SQLiteConnection database;

    public static GameDatabase Instance => instance;
    public SQLiteConnection Database => database;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        string databasePath = Path.Combine(
            Application.persistentDataPath,
            "StrawberryAndSteel.db"
        );

        database = new SQLiteConnection(databasePath);

        database.Execute("PRAGMA foreign_keys = ON;");

        CreateTables();
        InsertInitialItemTypes();
        InsertInitialItems();

        Debug.Log("SQLite database connected: " + databasePath);
        Debug.Log("Database initialized successfully.");
    }

    private void CreateTables()
    {
        // 1. Player
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS Player (
                PlayerId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );
        ");

        // 2. ItemTypes
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS ItemTypes (
                ItemTypeId INTEGER PRIMARY KEY,
                ItemTypeName TEXT NOT NULL UNIQUE
            );
        ");

        // 3. Item
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS Item (
                ItemId INTEGER PRIMARY KEY,
                ItemName TEXT NOT NULL,
                ItemTypeId INTEGER NOT NULL,
                ItemDescription TEXT,
                ItemValue INTEGER NOT NULL,
                IsStackable INTEGER NOT NULL,
                MaxStack INTEGER NOT NULL,

                FOREIGN KEY (ItemTypeId)
                    REFERENCES ItemTypes(ItemTypeId)
            );
        ");

        // 4. PlayerHasItem
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerHasItem (
                PlayerHasItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,

                UNIQUE (PlayerId, ItemId),
                UNIQUE (PlayerHasItemId, PlayerId),

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId),

                FOREIGN KEY (ItemId)
                    REFERENCES Item(ItemId)
            );
        ");

        // 5. PlayerProgress
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerProgress (
                PlayerProgressId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL UNIQUE,
                LastUnlockedLevel INTEGER NOT NULL,
                CurrentLevel INTEGER NOT NULL,

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
            );
        ");

        // 6. PlayerSettings
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerSettings (
                PlayerId INTEGER PRIMARY KEY,
                AudioLevel REAL NOT NULL,
                MusicLevel REAL NOT NULL,
                Sensitivity REAL NOT NULL,

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
            );
        ");

        // 7. PlayerEquipment
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerEquipment (
                PlayerEquipmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                PlayerHasItemId INTEGER NOT NULL,

                UNIQUE (PlayerId, PlayerHasItemId),

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId),

                FOREIGN KEY (PlayerHasItemId, PlayerId)
                    REFERENCES PlayerHasItem(PlayerHasItemId, PlayerId)
            );
        ");
    }

    private void InsertInitialItemTypes()
    {
        database.Execute(@"
            INSERT OR IGNORE INTO ItemTypes
            (ItemTypeId, ItemTypeName)
            VALUES
            (1, 'Collectable'),
            (2, 'Armor'),
            (3, 'Weapon'),
            (4, 'Magic');
        ");
    }

    private void InsertInitialItems()
    {
        database.Execute(@"
            INSERT OR IGNORE INTO Item
            (ItemId, ItemName, ItemTypeId, ItemDescription, ItemValue, IsStackable, MaxStack)
            VALUES

            -- Collectables
            (1, 'Coin', 1, 'Basic game currency.', 1, 1, 99999),
            (2, 'Diamond', 1, 'Rare game currency.', 10, 1, 9999),

            -- Swords
            (3, 'WoodenSword', 3, 'Basic wooden sword.', 10, 0, 1),
            (4, 'StoneSword', 3, 'Stone sword.', 20, 0, 1),
            (5, 'IronSword', 3, 'Iron sword.', 40, 0, 1),
            (6, 'SteelSword', 3, 'Steel sword.', 60, 0, 1),
            (7, 'SilverSword', 3, 'Silver sword.', 80, 0, 1),
            (8, 'GoldSword', 3, 'Gold sword.', 120, 0, 1),
            (9, 'CrystalSword', 3, 'Crystal sword.', 160, 0, 1),
            (10, 'DarkSword', 3, 'Dark sword.', 220, 0, 1),
            (11, 'DragonSword', 3, 'Dragon sword.', 300, 0, 1),
            (12, 'LegendarySword', 3, 'Legendary sword.', 500, 0, 1),

            -- Armors
            (13, 'ClothArmor', 2, 'Basic cloth armor.', 10, 0, 1),
            (14, 'LeatherArmor', 2, 'Leather armor.', 20, 0, 1),
            (15, 'WoodenArmor', 2, 'Wooden armor.', 30, 0, 1),
            (16, 'StoneArmor', 2, 'Stone armor.', 40, 0, 1),
            (17, 'IronArmor', 2, 'Iron armor.', 60, 0, 1),
            (18, 'SteelArmor', 2, 'Steel armor.', 90, 0, 1),
            (19, 'SilverArmor', 2, 'Silver armor.', 120, 0, 1),
            (20, 'GoldArmor', 2, 'Gold armor.', 160, 0, 1),
            (21, 'CrystalArmor', 2, 'Crystal armor.', 220, 0, 1),
            (22, 'PlatinumArmor', 2, 'Platinum armor.', 350, 0, 1),

            -- Arrows
            (23, 'WoodenArrow', 3, 'Basic wooden arrow.', 1, 1, 99),
            (24, 'StoneArrow', 3, 'Stone arrow.', 2, 1, 99),
            (25, 'IronArrow', 3, 'Iron arrow.', 3, 1, 99),
            (26, 'SteelArrow', 3, 'Steel arrow.', 5, 1, 99),
            (27, 'FireArrow', 3, 'Fire arrow.', 8, 1, 99),
            (28, 'IceArrow', 3, 'Ice arrow.', 8, 1, 99),
            (29, 'PoisonArrow', 3, 'Poison arrow.', 10, 1, 99),
            (30, 'ExplosiveArrow', 3, 'Explosive arrow.', 15, 1, 99),
            (31, 'CrystalArrow', 3, 'Crystal arrow.', 20, 1, 99),
            (32, 'DragonArrow', 3, 'Dragon arrow.', 30, 1, 99),

            -- Magic
            (33, 'Fireball', 4, 'Fire damage spell.', 30, 0, 1),
            (34, 'IceBlast', 4, 'Ice damage spell.', 30, 0, 1),
            (35, 'LightningStrike', 4, 'Lightning damage spell.', 40, 0, 1),
            (36, 'HealingLight', 4, 'Healing spell.', 25, 0, 1),
            (37, 'PoisonCloud', 4, 'Poison area spell.', 35, 0, 1),
            (38, 'WindSlash', 4, 'Wind damage spell.', 35, 0, 1),
            (39, 'EarthSpike', 4, 'Earth damage spell.', 40, 0, 1),
            (40, 'ShadowBolt', 4, 'Darkness damage spell.', 50, 0, 1),
            (41, 'HolyBeam', 4, 'Holy damage spell.', 60, 0, 1),
            (42, 'DragonFlame', 4, 'Powerful dragon fire spell.', 100, 0, 1);
        ");
    }

    private void OnDestroy()
    {
        if (instance != this)
            return;

        database?.Close();
        database?.Dispose();

        instance = null;
    }

    private void OnApplicationQuit()
    {
        database?.Close();
        database?.Dispose();
        database = null;
    }
}
