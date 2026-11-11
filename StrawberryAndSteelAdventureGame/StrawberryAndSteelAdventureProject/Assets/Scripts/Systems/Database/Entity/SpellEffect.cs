using SQLite;

[Table("SpellEffect")]
public class SpellEffect
{
    [PrimaryKey, AutoIncrement]
    public int SpellEffectId { get; set; }

    [NotNull]
    public int SpellId { get; set; }

    [NotNull]
    public string EffectType { get; set; }

    [NotNull]
    public float EffectValue { get; set; }

    [NotNull]
    public float Duration { get; set; }
}