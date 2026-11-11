using SQLite;

[Table("PlayerHasSpell")]
public class PlayerHasSpell
{
    [PrimaryKey, AutoIncrement]
    public int PlayerHasSpellId { get; set; }

    [NotNull]
    public int PlayerId { get; set; }

    [NotNull]
    public int SpellId { get; set; }
}