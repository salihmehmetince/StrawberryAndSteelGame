
using System;

public class PlayerCharacterSession
{
    public event Action OnStatsChanged;

    public PlayerCharacterLevelStats CharacterStats { get; private set; }

    // Health
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    // Mana
    public int MaxMana { get; private set; }
    public int CurrentMana { get; private set; }

    // XP
    public long SavedXP { get; private set; }
    public long CurrentXP { get; private set; }

    public long CurrentLevelStartXP { get; private set; }
    public long NextLevelRequiredXP { get; private set; }

    public bool IsDead => CurrentHealth <= 0;

    public void Initialize(PlayerCharacterLevelStats characterStats)
    {
        if (characterStats == null)
            throw new ArgumentNullException(nameof(characterStats));

        CharacterStats = characterStats;

        MaxHealth = characterStats.MaxHealth;
        CurrentHealth = MaxHealth;

        MaxMana = characterStats.MaxMana;
        CurrentMana = MaxMana;

        OnStatsChanged?.Invoke();
    }

    public void InitializeXP(
        long savedXP,
        long currentLevelStartXP,
        long nextLevelRequiredXP)
    {
        if (savedXP < 0 ||
            currentLevelStartXP < 0 ||
            nextLevelRequiredXP < 0)
            throw new ArgumentException("Invalid XP values.");

        SavedXP = savedXP;
        CurrentXP = savedXP;

        CurrentLevelStartXP = currentLevelStartXP;
        NextLevelRequiredXP = nextLevelRequiredXP;

        OnStatsChanged?.Invoke();
    }

    public void GainXP(long amount)
    {
        if (amount <= 0)
            return;

        try
        {
            CurrentXP = checked(CurrentXP + amount);
        }
        catch (OverflowException)
        {
            return;
        }

        OnStatsChanged?.Invoke();
    }

    public void MarkXPSaved()
    {
        SavedXP = CurrentXP;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead)
            return;

        CurrentHealth = Math.Max(0, CurrentHealth - damage);

        OnStatsChanged?.Invoke();
    }

    public void RestoreHealth(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);

        OnStatsChanged?.Invoke();
    }

    public bool UseMana(int amount)
    {
        if (amount <= 0 || CurrentMana < amount)
            return false;

        CurrentMana -= amount;

        OnStatsChanged?.Invoke();

        return true;
    }

    public void RestoreMana(int amount)
    {
        if (amount <= 0)
            return;

        CurrentMana = Math.Min(MaxMana, CurrentMana + amount);

        OnStatsChanged?.Invoke();
    }

    public void Clear()
    {
        CharacterStats = null;

        MaxHealth = 0;
        CurrentHealth = 0;

        MaxMana = 0;
        CurrentMana = 0;

        SavedXP = 0;
        CurrentXP = 0;

        CurrentLevelStartXP = 0;
        NextLevelRequiredXP = 0;

        OnStatsChanged?.Invoke();
    }
}
