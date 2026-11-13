
using SQLite;

[Table("CharacterLevelDefinition")]
public class CharacterLevelDefinition
{
    [PrimaryKey, AutoIncrement]
    public int CharacterLevelDefinitionId { get; set; }

    [NotNull, Unique]
    public int Level { get; set; }

    [NotNull, Unique]
    public int RequiredTotalXP { get; set; }

    [NotNull]
    public int BaseMaxHealth { get; set; } = 100;

    [NotNull]
    public int BaseMaxMana { get; set; } = 100;

    [NotNull]
    public int HealthIncrease { get; set; }

    [NotNull]
    public int ManaIncrease { get; set; }

    [NotNull]
    public int BaseStrength { get; set; } = 10;

    [NotNull]
    public int StrengthIncrease { get; set; }

    [NotNull]
    public float BaseMovementSpeed { get; set; } = 30f;

    [NotNull]
    public float MovementSpeedIncrease { get; set; }
}
