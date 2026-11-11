using System.Collections.Generic;

public class SpellService
{
    private readonly SpellRepository spellRepository;

    public SpellService(SpellRepository spellRepository)
    {
        this.spellRepository = spellRepository;
    }

    public Spell GetSpellById(int spellId)
    {
        if (spellId <= 0)
            return null;

        return spellRepository.GetById(spellId);
    }

    public List<Spell> GetAllSpells()
    {
        return spellRepository.GetAll();
    }

    public int CreateSpell(Spell spell)
    {
        if (spell == null)
            return 0;

        return spellRepository.Add(spell);
    }

    public bool UpdateSpell(Spell spell)
    {
        if (spell == null || spell.SpellId <= 0)
            return false;

        return spellRepository.Update(spell);
    }

    public bool DeleteSpell(int spellId)
    {
        if (spellId <= 0)
            return false;

        return spellRepository.Delete(spellId);
    }

    public bool SpellExists(int spellId)
    {
        if (spellId <= 0)
            return false;

        return spellRepository.Exists(spellId);
    }

    public int GetSpellCount()
    {
        return spellRepository.Count();
    }
}