
using System.Collections.Generic;

public class PlayerCharacterLevelStatsService
{
    private readonly PlayerCharacterStatsService characterStatsService;
    private readonly CharacterLevelDefinitionService levelDefinitionService;
    private readonly PlayerEquipmentService equipmentService;
    private readonly PlayerHasItemService playerHasItemService;
    private readonly ItemService itemService;
    private readonly ItemStatModifierService modifierService;

    public PlayerCharacterLevelStatsService(
        PlayerCharacterStatsService characterStatsService,
        CharacterLevelDefinitionService levelDefinitionService,
        PlayerEquipmentService equipmentService,
        PlayerHasItemService playerHasItemService,
        ItemService itemService,
        ItemStatModifierService modifierService)
    {
        this.characterStatsService = characterStatsService;
        this.levelDefinitionService = levelDefinitionService;
        this.equipmentService = equipmentService;
        this.playerHasItemService = playerHasItemService;
        this.itemService = itemService;
        this.modifierService = modifierService;
    }


    public PlayerCharacterLevelStats GetLevelStats(int playerId)
    {
        if (playerId <= 0)
        {
            UnityEngine.Debug.LogError(
                "GetLevelStats: Invalid PlayerId: " + playerId
            );
            return null;
        }

        PlayerCharacterStats characterStats =
            characterStatsService.GetStats(playerId);

        if (characterStats == null)
        {
            UnityEngine.Debug.LogError(
                "GetLevelStats: PlayerCharacterStats not found. PlayerId: " + playerId
            );
            return null;
        }

        UnityEngine.Debug.Log(
            "GetLevelStats: TotalXP = " + characterStats.TotalXP
        );

        CharacterLevelDefinition definition =
            levelDefinitionService.GetDefinitionByTotalXP(
                characterStats.TotalXP
            );

        if (definition == null)
        {
            UnityEngine.Debug.LogError(
                "GetLevelStats: CharacterLevelDefinition not found. TotalXP: " +
                characterStats.TotalXP
            );
            return null;
        }

        int baseHealth =
            definition.BaseMaxHealth + definition.HealthIncrease;

        int baseMana =
            definition.BaseMaxMana + definition.ManaIncrease;

        float baseStrength =
            definition.BaseStrength + definition.StrengthIncrease;

        float baseMovementSpeed =
            definition.BaseMovementSpeed +
            definition.MovementSpeedIncrease;

        float totalDefense =
            GetEquipmentModifier(playerId, 1, "Defense") +
            GetEquipmentModifier(playerId, 2, "Defense") +
            GetEquipmentModifier(playerId, 3, "Defense") +
            GetEquipmentModifier(playerId, 4, "Defense") +
            GetEquipmentModifier(playerId, 5, "Defense") +
            GetEquipmentModifier(playerId, 6, "Defense");

        float swordAttack =
            GetEquipmentModifier(playerId, 7, "Attack");

        float weaponAttack =
            GetEquipmentModifier(playerId, 8, "Attack");

        return new PlayerCharacterLevelStats
        {
            Level = definition.Level,
            MaxHealth = baseHealth + (int)totalDefense,
            MaxMana = baseMana,
            SwordPower = baseStrength + swordAttack,
            WeaponPower = baseStrength + weaponAttack,
            MovementSpeed = baseMovementSpeed
        };
    }
    // Mevcut metodun geri kalan kýsmý burada ayný kalacak.
    private float GetEquipmentModifier(
        int playerId,
        int equipmentSlotId,
        string statType)
    {
        PlayerEquipment equipment =
            equipmentService.GetBySlot(playerId, equipmentSlotId);

        if (equipment == null)
            return 0f;

        PlayerHasItem playerItem =
            playerHasItemService.GetById(equipment.PlayerHasItemId);

        if (playerItem == null || playerItem.PlayerId != playerId)
            return 0f;

        Item item =
            itemService.GetItemById(playerItem.ItemId);

        if (item == null)
            return 0f;

        List<ItemStatModifier> modifiers =
            modifierService.GetModifiersByItemId(item.ItemId);

        if (modifiers == null)
            return 0f;

        float totalModifier = 0f;

        foreach (ItemStatModifier modifier in modifiers)
        {
            if (modifier.StatType == statType)
                totalModifier += modifier.ModifierValue;
        }

        return totalModifier;
    }
}
