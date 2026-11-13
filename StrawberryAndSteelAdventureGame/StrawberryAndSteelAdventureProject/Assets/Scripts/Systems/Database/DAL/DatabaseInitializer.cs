
using System;
using SQLite;

public static class DatabaseInitializer
{
    private const int CurrentSchemaVersion = 6;

    public static void Initialize(DatabaseContext context)
    {
        SQLiteConnection database = context.Connection;

        CreateSchemaVersionTable(database);

        int currentVersion = database.ExecuteScalar<int>(
            "SELECT COALESCE(MAX(Version), 0) FROM DatabaseSchemaVersion"
        );

        if (currentVersion > CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                "Database version is newer than the supported version."
            );
        }

        if (currentVersion < CurrentSchemaVersion)
        {
            database.RunInTransaction(() =>
            {
                DropTables(database);
                CreateTables(database);
                SeedData(database);

                database.Execute(
                    "DELETE FROM DatabaseSchemaVersion"
                );

                database.Insert(new DatabaseSchemaVersion
                {
                    DatabaseSchemaVersionId = 1,
                    Version = CurrentSchemaVersion,
                    UpdatedAt = DateTime.UtcNow.ToShortDateString()
                });
            });
        }
        else
        {
            CreateTables(database);
        }

        UnityEngine.Debug.Log(
            "Database initialization completed. Version: " +
            CurrentSchemaVersion
        );
    }

    // ==================================================
    // DATABASE SCHEMA VERSION
    // ==================================================

    private static void CreateSchemaVersionTable(
        SQLiteConnection database)
    {
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS DatabaseSchemaVersion
            (
                DatabaseSchemaVersionId INTEGER PRIMARY KEY,
                Version INTEGER NOT NULL,
                UpdatedAt TEXT NOT NULL
            );
        ");
    }

    // ==================================================
    // DROP TABLES
    // ==================================================

    private static void DropTables(SQLiteConnection database)
    {
        database.Execute("DROP TABLE IF EXISTS PlayerActiveSpell;");
        database.Execute("DROP TABLE IF EXISTS PlayerHasSpell;");
        database.Execute("DROP TABLE IF EXISTS PlayerEquipment;");
        database.Execute("DROP TABLE IF EXISTS PlayerLevelCompletion;");
        database.Execute("DROP TABLE IF EXISTS PlayerCharacterStats;");
        database.Execute("DROP TABLE IF EXISTS PlayerHasItem;");
        database.Execute("DROP TABLE IF EXISTS PlayerProgress;");
        database.Execute("DROP TABLE IF EXISTS ShopOffer;");
        database.Execute("DROP TABLE IF EXISTS SpellEffect;");
        database.Execute("DROP TABLE IF EXISTS ItemStatModifier;");
        database.Execute("DROP TABLE IF EXISTS Spell;");
        database.Execute("DROP TABLE IF EXISTS Item;");
        database.Execute("DROP TABLE IF EXISTS CharacterLevelDefinition;");
        database.Execute("DROP TABLE IF EXISTS EquipmentSlot;");
        database.Execute("DROP TABLE IF EXISTS ItemTypes;");
        database.Execute("DROP TABLE IF EXISTS GameSettings;");
        database.Execute("DROP TABLE IF EXISTS PlayerSettings;");
        database.Execute("DROP TABLE IF EXISTS Player;");
    }

    // ==================================================
    // CREATE TABLES
    // ==================================================

    private static void CreateTables(SQLiteConnection database)
    {
        database.Execute(@"
            CREATE TABLE IF NOT EXISTS Player
            (
                PlayerId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS GameSettings
            (
                GameSettingsId INTEGER PRIMARY KEY
                    CHECK (GameSettingsId = 1),
                AudioLevel REAL NOT NULL
                    CHECK (AudioLevel BETWEEN 0 AND 1),
                MusicLevel REAL NOT NULL
                    CHECK (MusicLevel BETWEEN 0 AND 1),
                Sensitivity REAL NOT NULL
                    CHECK (Sensitivity BETWEEN 0 AND 1)
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS ItemTypes
            (
                ItemTypeId INTEGER PRIMARY KEY,
                ItemTypeName TEXT NOT NULL UNIQUE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS EquipmentSlot
            (
                EquipmentSlotId INTEGER PRIMARY KEY AUTOINCREMENT,
                SlotName TEXT NOT NULL UNIQUE
            );
        ");

        database.Execute(@"
    CREATE TABLE IF NOT EXISTS CharacterLevelDefinition
    (
        CharacterLevelDefinitionId INTEGER PRIMARY KEY AUTOINCREMENT,
        Level INTEGER NOT NULL UNIQUE,
        RequiredTotalXP INTEGER NOT NULL UNIQUE,
        BaseMaxHealth INTEGER NOT NULL,
        BaseMaxMana INTEGER NOT NULL,
        HealthIncrease INTEGER NOT NULL,
        ManaIncrease INTEGER NOT NULL,
        BaseStrength INTEGER NOT NULL,
        StrengthIncrease INTEGER NOT NULL,
        BaseMovementSpeed REAL NOT NULL,
        MovementSpeedIncrease REAL NOT NULL
    );
");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS Item
            (
                ItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                ItemName TEXT NOT NULL UNIQUE,
                ItemTypeId INTEGER NOT NULL,
                EquipmentSlotId INTEGER,
                ItemDescription TEXT,
                IsActive INTEGER NOT NULL
                    CHECK (IsActive IN (0, 1)),

                FOREIGN KEY (ItemTypeId)
                    REFERENCES ItemTypes(ItemTypeId),

                FOREIGN KEY (EquipmentSlotId)
                    REFERENCES EquipmentSlot(EquipmentSlotId)
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS Spell
            (
                SpellId INTEGER PRIMARY KEY AUTOINCREMENT,
                SpellName TEXT NOT NULL UNIQUE,
                SpellDescription TEXT,
                ManaCost INTEGER NOT NULL,
                Cooldown REAL NOT NULL,
                BaseDamage REAL NOT NULL,
                IsActive INTEGER NOT NULL
                    CHECK (IsActive IN (0, 1))
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerProgress
            (
                PlayerProgressId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL UNIQUE,
                LastUnlockedLevel INTEGER NOT NULL,
                CurrentLevel INTEGER NOT NULL,

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
                    ON DELETE CASCADE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerCharacterStats
            (
                PlayerCharacterStatsId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL UNIQUE,
                TotalXP INTEGER NOT NULL,

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
                    ON DELETE CASCADE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerHasItem
            (
                PlayerHasItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL
                    CHECK (Quantity >= 0),

                UNIQUE (PlayerId, ItemId),
                UNIQUE (PlayerHasItemId, PlayerId),

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
                    ON DELETE CASCADE,

                FOREIGN KEY (ItemId)
                    REFERENCES Item(ItemId)
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS ItemStatModifier
            (
                ItemStatModifierId INTEGER PRIMARY KEY AUTOINCREMENT,
                ItemId INTEGER NOT NULL,
                StatType TEXT NOT NULL,
                ModifierValue REAL NOT NULL,

                UNIQUE (ItemId, StatType),

                FOREIGN KEY (ItemId)
                    REFERENCES Item(ItemId)
                    ON DELETE CASCADE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS SpellEffect
            (
                SpellEffectId INTEGER PRIMARY KEY AUTOINCREMENT,
                SpellId INTEGER NOT NULL,
                EffectType TEXT NOT NULL,
                EffectValue REAL NOT NULL,
                Duration REAL NOT NULL,

                UNIQUE (SpellId, EffectType),

                FOREIGN KEY (SpellId)
                    REFERENCES Spell(SpellId)
                    ON DELETE CASCADE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerEquipment
            (
                PlayerEquipmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                PlayerHasItemId INTEGER NOT NULL,
                EquipmentSlotId INTEGER NOT NULL,

                UNIQUE (PlayerId, EquipmentSlotId),
                UNIQUE (PlayerId, PlayerHasItemId),

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
                    ON DELETE CASCADE,

                FOREIGN KEY (PlayerHasItemId, PlayerId)
                    REFERENCES PlayerHasItem(PlayerHasItemId, PlayerId)
                    ON DELETE CASCADE,

                FOREIGN KEY (EquipmentSlotId)
                    REFERENCES EquipmentSlot(EquipmentSlotId)
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerHasSpell
            (
                PlayerHasSpellId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                SpellId INTEGER NOT NULL,

                UNIQUE (PlayerId, SpellId),

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
                    ON DELETE CASCADE,

                FOREIGN KEY (SpellId)
                    REFERENCES Spell(SpellId)
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerActiveSpell
            (
                PlayerActiveSpellId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                SpellId INTEGER NOT NULL,
                SpellSlot INTEGER NOT NULL
                    CHECK (SpellSlot BETWEEN 1 AND 3),

                UNIQUE (PlayerId, SpellId),
                UNIQUE (PlayerId, SpellSlot),

                FOREIGN KEY (PlayerId, SpellId)
                    REFERENCES PlayerHasSpell(PlayerId, SpellId)
                    ON DELETE CASCADE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS PlayerLevelCompletion
            (
                PlayerLevelCompletionId INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerId INTEGER NOT NULL,
                LevelNumber INTEGER NOT NULL,
                CompletedAt TEXT NOT NULL,

                UNIQUE (PlayerId, LevelNumber),

                FOREIGN KEY (PlayerId)
                    REFERENCES Player(PlayerId)
                    ON DELETE CASCADE
            );
        ");

        database.Execute(@"
            CREATE TABLE IF NOT EXISTS ShopOffer
            (
                ShopOfferId INTEGER PRIMARY KEY AUTOINCREMENT,
                ItemId INTEGER,
                SpellId INTEGER,
                CoinPrice INTEGER NOT NULL
                    CHECK (CoinPrice >= 0),
                DiamondPrice INTEGER NOT NULL
                    CHECK (DiamondPrice >= 0),
                IsActive INTEGER NOT NULL
                    CHECK (IsActive IN (0, 1)),

                CHECK (
                    (ItemId IS NOT NULL AND SpellId IS NULL)
                    OR
                    (ItemId IS NULL AND SpellId IS NOT NULL)
                ),

                FOREIGN KEY (ItemId)
                    REFERENCES Item(ItemId),

                FOREIGN KEY (SpellId)
                    REFERENCES Spell(SpellId)
            );
        ");
    }

    // ==================================================
    // SEED DATA
    // ==================================================

    private static void SeedData(SQLiteConnection database)
    {
        InsertGameSettings(database);
        InsertItemTypes(database);
        InsertEquipmentSlots(database);
        InsertCharacterLevels(database);
        InsertSpells(database);
        InsertItems(database);
        InsertShopOffers(database);
    }

    // ==================================================
    // GAME SETTINGS
    // ==================================================

    private static void InsertGameSettings(SQLiteConnection database)
    {
        database.Insert(new GameSettings
        {
            GameSettingsId = 1,
            AudioLevel = 1f,
            MusicLevel = 1f,
            Sensitivity = 1f
        });
    }

    // ==================================================
    // ITEM TYPES
    // ==================================================

    private static void InsertItemTypes(SQLiteConnection database)
    {
        database.Insert(new ItemType
        {
            ItemTypeId = 1,
            ItemTypeName = "Collectable"
        });

        database.Insert(new ItemType
        {
            ItemTypeId = 2,
            ItemTypeName = "Armor"
        });

        database.Insert(new ItemType
        {
            ItemTypeId = 3,
            ItemTypeName = "Weapon"
        });

        database.Insert(new ItemType
        {
            ItemTypeId = 4,
            ItemTypeName = "Magic"
        });
    }

    // ==================================================
    // EQUIPMENT SLOTS
    // ==================================================

    private static void InsertEquipmentSlots(SQLiteConnection database)
    {
        string[] slots =
        {
            "Helmet",
            "Bracers",
            "Chest",
            "Legs",
            "Boots",
            "Shield",
            "Sword",
            "Weapon"
        };

        for (int i = 0; i < slots.Length; i++)
        {
            database.Insert(new EquipmentSlot
            {
                EquipmentSlotId = i + 1,
                SlotName = slots[i]
            });
        }
    }

    // ==================================================
    // CHARACTER LEVELS
    // ==================================================

    private static void InsertCharacterLevels(SQLiteConnection database)
    {
        int[] requiredXP =
        {
        0, 100, 300, 600, 1000,
        1500, 2100, 2800, 3600, 4500,
        5500, 6600, 7800, 9100, 10500,
        12000, 13600, 15300, 17100, 19000
    };

        for (int level = 1; level <= 20; level++)
        {
            database.Insert(new CharacterLevelDefinition
            {
                CharacterLevelDefinitionId = level,
                Level = level,
                RequiredTotalXP = requiredXP[level - 1],

                BaseMaxHealth = 100,
                BaseMaxMana = 100,
                HealthIncrease = (level - 1) * 10,
                ManaIncrease = (level - 1) * 5,

                BaseStrength = 10,
                StrengthIncrease = (level - 1) * 5,

                BaseMovementSpeed = 30f,
                MovementSpeedIncrease = (level - 1) * 1f
            });
        }
    }
    // ==================================================
    // SPELLS
    // ==================================================

    private static void InsertSpells(SQLiteConnection database)
    {
        Spell[] spells =
        {
            new Spell
            {
                SpellName = "Fireball",
                SpellDescription = "Launches a fireball that burns enemies.",
                ManaCost = 15,
                Cooldown = 2f,
                BaseDamage = 35f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "IceBlast",
                SpellDescription = "Slows enemies with an ice blast.",
                ManaCost = 18,
                Cooldown = 3f,
                BaseDamage = 25f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "LightningStrike",
                SpellDescription = "Strikes enemies with powerful lightning.",
                ManaCost = 25,
                Cooldown = 4f,
                BaseDamage = 45f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "HealingLight",
                SpellDescription = "Restores the player's health.",
                ManaCost = 20,
                Cooldown = 5f,
                BaseDamage = 0f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "PoisonCloud",
                SpellDescription = "Creates a poisonous cloud.",
                ManaCost = 22,
                Cooldown = 5f,
                BaseDamage = 10f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "WindSlash",
                SpellDescription = "Launches a sharp blade of wind.",
                ManaCost = 12,
                Cooldown = 1.5f,
                BaseDamage = 30f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "EarthSpike",
                SpellDescription = "Summons a spike from the ground.",
                ManaCost = 20,
                Cooldown = 3.5f,
                BaseDamage = 40f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "ShadowBolt",
                SpellDescription = "Launches a dark projectile that steals life.",
                ManaCost = 20,
                Cooldown = 2.5f,
                BaseDamage = 38f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "HolyBeam",
                SpellDescription = "Fires a powerful beam of holy energy.",
                ManaCost = 30,
                Cooldown = 5f,
                BaseDamage = 55f,
                IsActive = 1
            },
            new Spell
            {
                SpellName = "DragonFlame",
                SpellDescription = "Unleashes powerful dragon fire.",
                ManaCost = 40,
                Cooldown = 8f,
                BaseDamage = 70f,
                IsActive = 1
            }
        };

        foreach (Spell spell in spells)
        {
            database.Insert(spell);
        }

        InsertSpellEffects(database);
    }

    // ==================================================
    // SPELL EFFECTS
    // ==================================================

    private static void InsertSpellEffects(SQLiteConnection database)
    {
        AddSpellEffect(database, "Fireball", "Burn", 8f, 3f);
        AddSpellEffect(database, "IceBlast", "Slow", 40f, 3f);
        AddSpellEffect(database, "LightningStrike", "Stun", 1f, 1f);
        AddSpellEffect(database, "HealingLight", "Heal", 40f, 0f);
        AddSpellEffect(database, "PoisonCloud", "Poison", 6f, 5f);
        AddSpellEffect(database, "WindSlash", "Knockback", 5f, 0f);
        AddSpellEffect(database, "EarthSpike", "Slow", 30f, 2f);
        AddSpellEffect(database, "ShadowBolt", "LifeSteal", 20f, 0f);
        AddSpellEffect(database, "HolyBeam", "Damage", 55f, 0f);
        AddSpellEffect(database, "DragonFlame", "Burn", 15f, 4f);
    }

    private static void AddSpellEffect(
        SQLiteConnection database,
        string spellName,
        string effectType,
        float effectValue,
        float duration)
    {
        int spellId = database.ExecuteScalar<int>(
            "SELECT SpellId FROM Spell WHERE SpellName = ?",
            spellName
        );

        database.Insert(new SpellEffect
        {
            SpellId = spellId,
            EffectType = effectType,
            EffectValue = effectValue,
            Duration = duration
        });
    }

    // ==================================================
    // ITEMS
    // ==================================================

    private static void InsertItems(SQLiteConnection database)
    {
        AddItem(
            database,
            "Coin",
            1,
            null,
            "Basic game currency."
        );

        AddItem(
            database,
            "Diamond",
            1,
            null,
            "Rare game currency."
        );

        InsertArmorItems(database);
        InsertSwordItems(database);
        InsertBowItems(database);
        InsertAdditionalWeaponItems(database);
    }

    // ==================================================
    // ARMOR AND SHIELDS
    // ==================================================

    private static void InsertArmorItems(SQLiteConnection database)
    {
        string[] materials =
        {
            "Cloth",
            "Leather",
            "Wooden",
            "Stone",
            "Iron",
            "Steel",
            "Silver",
            "Gold",
            "Crystal",
            "Platinum"
        };

        int[] materialDefense =
        {
            1, 2, 2, 3, 4, 5, 6, 7, 8, 10
        };

        string[] armorParts =
        {
            "Helmet",
            "Bracers",
            "Chest",
            "Legs",
            "Boots",
            "Shield"
        };

        int[] slotIds =
        {
            1, 2, 3, 4, 5, 6
        };

        int[] defenseMultipliers =
        {
            2, 2, 5, 3, 2, 6
        };

        for (int materialIndex = 0;
             materialIndex < materials.Length;
             materialIndex++)
        {
            for (int partIndex = 0;
                 partIndex < armorParts.Length;
                 partIndex++)
            {
                string itemName =
                    materials[materialIndex] + armorParts[partIndex];

                int defense =
                    materialDefense[materialIndex] *
                    defenseMultipliers[partIndex];

                AddEquipmentItem(
                    database,
                    itemName,
                    2,
                    slotIds[partIndex],
                    materials[materialIndex] + " " +
                    armorParts[partIndex] + ".",
                    "Defense",
                    defense
                );
            }
        }
    }

    // ==================================================
    // SWORDS
    // ==================================================

    private static void InsertSwordItems(SQLiteConnection database)
    {
        string[] materials =
        {
            "Wooden",
            "Stone",
            "Iron",
            "Steel",
            "Silver",
            "Gold",
            "Crystal",
            "Dark",
            "Dragon",
            "Legendary"
        };

        int[] attackValues =
        {
            5, 10, 15, 20, 25,
            30, 38, 42, 50, 60
        };

        for (int i = 0; i < materials.Length; i++)
        {
            AddEquipmentItem(
                database,
                materials[i] + "Sword",
                3,
                7,
                materials[i] + " sword.",
                "Attack",
                attackValues[i]
            );
        }
    }

    // ==================================================
    // BOWS
    // ==================================================

    private static void InsertBowItems(SQLiteConnection database)
    {
        string[] materials =
        {
            "Wooden",
            "Stone",
            "Iron",
            "Steel",
            "Silver",
            "Gold",
            "Crystal",
            "Dark",
            "Dragon",
            "Legendary"
        };

        int[] attackValues =
        {
            4, 8, 12, 16, 20,
            24, 30, 34, 42, 50
        };

        for (int i = 0; i < materials.Length; i++)
        {
            AddEquipmentItem(
                database,
                materials[i] + "Bow",
                3,
                8,
                materials[i] + " bow with built-in unlimited ammunition. No separate ammunition is required.",
                "Attack",
                attackValues[i]
            );
        }
    }

    // ==================================================
    // ADDITIONAL WEAPONS
    // ==================================================

    private static void InsertAdditionalWeaponItems(
        SQLiteConnection database)
    {
        string[] weaponNames =
        {
            "Dagger",
            "Axe",
            "Spear",
            "Hammer",
            "Scythe",
            "Katana",
            "Rapier",
            "Mace",
            "Crossbow",
            "Staff"
        };

        int[] attackValues =
        {
            12, 22, 20, 28, 32,
            30, 24, 26, 25, 18
        };

        for (int i = 0; i < weaponNames.Length; i++)
        {
            AddEquipmentItem(
                database,
                weaponNames[i],
                3,
                8,
                weaponNames[i] == "Crossbow"
                    ? "Crossbow with built-in unlimited ammunition. No separate ammunition is required."
                    : weaponNames[i] + " weapon.",
                "Attack",
                attackValues[i]
            );
        }
    }

    // ==================================================
    // ITEM HELPERS
    // ==================================================

    private static void AddItem(
        SQLiteConnection database,
        string itemName,
        int itemTypeId,
        int? equipmentSlotId,
        string description)
    {
        database.Insert(new Item
        {
            ItemName = itemName,
            ItemTypeId = itemTypeId,
            EquipmentSlotId = equipmentSlotId,
            ItemDescription = description,
            IsActive = 1
        });
    }

    private static void AddEquipmentItem(
        SQLiteConnection database,
        string itemName,
        int itemTypeId,
        int equipmentSlotId,
        string description,
        string statType,
        float modifierValue)
    {
        Item item = new Item
        {
            ItemName = itemName,
            ItemTypeId = itemTypeId,
            EquipmentSlotId = equipmentSlotId,
            ItemDescription = description,
            IsActive = 1
        };

        database.Insert(item);

        database.Insert(new ItemStatModifier
        {
            ItemId = item.ItemId,
            StatType = statType,
            ModifierValue = modifierValue
        });
    }

    // ==================================================
    // SHOP OFFERS
    // ==================================================

    private static void InsertShopOffers(SQLiteConnection database)
    {
        InsertArmorShopOffers(database);
        InsertSwordShopOffers(database);
        InsertBowShopOffers(database);
        InsertAdditionalWeaponShopOffers(database);
        InsertSpellShopOffers(database);
    }

    // --------------------------------------------------
    // ARMOR SHOP OFFERS
    // --------------------------------------------------

    private static void InsertArmorShopOffers(
        SQLiteConnection database)
    {
        string[] materials =
        {
            "Cloth",
            "Leather",
            "Wooden",
            "Stone",
            "Iron",
            "Steel",
            "Silver",
            "Gold",
            "Crystal",
            "Platinum"
        };

        int[] coinPrices =
        {
            20, 40, 60, 100, 180,
            300, 450, 650, 900, 1300
        };

        int[] diamondPrices =
        {
            0, 0, 0, 0, 1,
            2, 3, 4, 6, 8
        };

        string[] armorParts =
        {
            "Helmet",
            "Bracers",
            "Chest",
            "Legs",
            "Boots",
            "Shield"
        };

        for (int materialIndex = 0;
             materialIndex < materials.Length;
             materialIndex++)
        {
            for (int partIndex = 0;
                 partIndex < armorParts.Length;
                 partIndex++)
            {
                string itemName =
                    materials[materialIndex] + armorParts[partIndex];

                AddItemShopOffer(
                    database,
                    itemName,
                    coinPrices[materialIndex],
                    diamondPrices[materialIndex]
                );
            }
        }
    }

    // --------------------------------------------------
    // SWORD SHOP OFFERS
    // --------------------------------------------------

    private static void InsertSwordShopOffers(
        SQLiteConnection database)
    {
        string[] swordNames =
        {
            "WoodenSword",
            "StoneSword",
            "IronSword",
            "SteelSword",
            "SilverSword",
            "GoldSword",
            "CrystalSword",
            "DarkSword",
            "DragonSword",
            "LegendarySword"
        };

        int[] coinPrices =
        {
            50, 100, 180, 280, 400,
            550, 750, 950, 1300, 1800
        };

        int[] diamondPrices =
        {
            0, 0, 1, 1, 2,
            3, 4, 5, 7, 10
        };

        for (int i = 0; i < swordNames.Length; i++)
        {
            AddItemShopOffer(
                database,
                swordNames[i],
                coinPrices[i],
                diamondPrices[i]
            );
        }
    }

    // --------------------------------------------------
    // BOW SHOP OFFERS
    // --------------------------------------------------

    private static void InsertBowShopOffers(
        SQLiteConnection database)
    {
        string[] bowNames =
        {
            "WoodenBow",
            "StoneBow",
            "IronBow",
            "SteelBow",
            "SilverBow",
            "GoldBow",
            "CrystalBow",
            "DarkBow",
            "DragonBow",
            "LegendaryBow"
        };

        int[] coinPrices =
        {
            45, 90, 160, 250, 360,
            500, 700, 900, 1200, 1700
        };

        int[] diamondPrices =
        {
            0, 0, 1, 1, 2,
            3, 4, 5, 7, 10
        };

        for (int i = 0; i < bowNames.Length; i++)
        {
            AddItemShopOffer(
                database,
                bowNames[i],
                coinPrices[i],
                diamondPrices[i]
            );
        }
    }

    // --------------------------------------------------
    // ADDITIONAL WEAPON SHOP OFFERS
    // --------------------------------------------------

    private static void InsertAdditionalWeaponShopOffers(
        SQLiteConnection database)
    {
        string[] weaponNames =
        {
            "Dagger",
            "Axe",
            "Spear",
            "Hammer",
            "Scythe",
            "Katana",
            "Rapier",
            "Mace",
            "Crossbow",
            "Staff"
        };

        int[] coinPrices =
        {
            120, 220, 200, 280, 350,
            400, 320, 300, 380, 250
        };

        int[] diamondPrices =
        {
            0, 1, 1, 1, 2,
            2, 1, 1, 2, 1
        };

        for (int i = 0; i < weaponNames.Length; i++)
        {
            AddItemShopOffer(
                database,
                weaponNames[i],
                coinPrices[i],
                diamondPrices[i]
            );
        }
    }

    // --------------------------------------------------
    // SPELL SHOP OFFERS
    // --------------------------------------------------

    private static void InsertSpellShopOffers(
        SQLiteConnection database)
    {
        string[] spellNames =
        {
            "Fireball",
            "IceBlast",
            "LightningStrike",
            "HealingLight",
            "PoisonCloud",
            "WindSlash",
            "EarthSpike",
            "ShadowBolt",
            "HolyBeam",
            "DragonFlame"
        };

        int[] coinPrices =
        {
            150, 200, 300, 250, 350,
            150, 250, 400, 550, 800
        };

        int[] diamondPrices =
        {
            0, 0, 1, 0, 1,
            0, 1, 2, 3, 5
        };

        for (int i = 0; i < spellNames.Length; i++)
        {
            AddSpellShopOffer(
                database,
                spellNames[i],
                coinPrices[i],
                diamondPrices[i]
            );
        }
    }

    // --------------------------------------------------
    // SHOP OFFER HELPERS
    // --------------------------------------------------

    private static void AddItemShopOffer(
        SQLiteConnection database,
        string itemName,
        int coinPrice,
        int diamondPrice)
    {
        int itemId = database.ExecuteScalar<int>(
            "SELECT ItemId FROM Item WHERE ItemName = ?",
            itemName
        );

        if (itemId <= 0)
        {
            throw new InvalidOperationException(
                "Shop item not found: " + itemName
            );
        }

        database.Insert(new ShopOffer
        {
            ItemId = itemId,
            SpellId = null,
            CoinPrice = coinPrice,
            DiamondPrice = diamondPrice,
            IsActive = 1
        });
    }

    private static void AddSpellShopOffer(
        SQLiteConnection database,
        string spellName,
        int coinPrice,
        int diamondPrice)
    {
        int spellId = database.ExecuteScalar<int>(
            "SELECT SpellId FROM Spell WHERE SpellName = ?",
            spellName
        );

        if (spellId <= 0)
        {
            throw new InvalidOperationException(
                "Shop spell not found: " + spellName
            );
        }

        database.Insert(new ShopOffer
        {
            ItemId = null,
            SpellId = spellId,
            CoinPrice = coinPrice,
            DiamondPrice = diamondPrice,
            IsActive = 1
        });
    }
}
