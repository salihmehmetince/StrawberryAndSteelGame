using SQLite;

[Table("PlayerActiveSpell")]
public class PlayerActiveSpell
{
    [PrimaryKey, AutoIncrement]
    public int PlayerActiveSpellId { get; set; }

    [NotNull]
    public int PlayerId { get; set; }

    [NotNull]
    public int SpellId { get; set; }

    [NotNull]
    public int SpellSlot { get; set; }
}