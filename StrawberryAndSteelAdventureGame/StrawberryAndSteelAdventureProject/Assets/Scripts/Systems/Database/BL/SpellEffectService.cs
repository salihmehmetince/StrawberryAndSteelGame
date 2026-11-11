using System.Collections.Generic;

public class SpellEffectService
{
    private readonly SpellEffectRepository spellEffectRepository;

    public SpellEffectService(SpellEffectRepository spellEffectRepository)
    {
        this.spellEffectRepository = spellEffectRepository;
    }

    public SpellEffect GetEffectById(int spellEffectId)
    {
        if (spellEffectId <= 0)
            return null;

        return spellEffectRepository.GetById(spellEffectId);
    }

    public List<SpellEffect> GetEffectsBySpellId(int spellId)
    {
        if (spellId <= 0)
            return new List<SpellEffect>();

        return spellEffectRepository.GetBySpellId(spellId);
    }

    public List<SpellEffect> GetAllEffects()
    {
        return spellEffectRepository.GetAll();
    }

    public int CreateEffect(SpellEffect spellEffect)
    {
        if (spellEffect == null || spellEffect.SpellId <= 0)
            return 0;

        return spellEffectRepository.Add(spellEffect);
    }

    public bool UpdateEffect(SpellEffect spellEffect)
    {
        if (spellEffect == null ||
            spellEffect.SpellEffectId <= 0 ||
            spellEffect.SpellId <= 0)
            return false;

        return spellEffectRepository.Update(spellEffect);
    }

    public bool DeleteEffect(int spellEffectId)
    {
        if (spellEffectId <= 0)
            return false;

        return spellEffectRepository.Delete(spellEffectId);
    }

    public int DeleteEffectsBySpellId(int spellId)
    {
        if (spellId <= 0)
            return 0;

        return spellEffectRepository.DeleteBySpellId(spellId);
    }

    public bool EffectExists(int spellEffectId)
    {
        if (spellEffectId <= 0)
            return false;

        return spellEffectRepository.Exists(spellEffectId);
    }

    public int GetEffectCount(int spellId)
    {
        if (spellId <= 0)
            return 0;

        return spellEffectRepository.CountBySpellId(spellId);
    }
}