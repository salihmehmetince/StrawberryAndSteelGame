using SQLite;

[Table("Spell")]
public class Spell
{
    [PrimaryKey, AutoIncrement]
    public int SpellId { get; set; }

    [NotNull, Unique]
    public string SpellName { get; set; }

    public string SpellDescription { get; set; }

    [NotNull]
    public int ManaCost { get; set; }

    [NotNull]
    public float Cooldown { get; set; }

    [NotNull]
    public float BaseDamage { get; set; }

    [NotNull]
    public int IsActive { get; set; }
}