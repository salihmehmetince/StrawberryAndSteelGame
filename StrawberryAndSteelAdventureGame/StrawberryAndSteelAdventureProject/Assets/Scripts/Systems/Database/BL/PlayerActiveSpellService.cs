using System.Collections.Generic;

public class PlayerActiveSpellService
{
    private const int MaxActiveSpellSlots = 3;

    private readonly PlayerActiveSpellRepository playerActiveSpellRepository;
    private readonly PlayerHasSpellService playerHasSpellService;

    public PlayerActiveSpellService(
        PlayerActiveSpellRepository playerActiveSpellRepository,
        PlayerHasSpellService playerHasSpellService)
    {
        this.playerActiveSpellRepository = playerActiveSpellRepository;
        this.playerHasSpellService = playerHasSpellService;
    }

    public PlayerActiveSpell GetById(int playerActiveSpellId)
    {
        if (playerActiveSpellId <= 0)
            return null;

        return playerActiveSpellRepository.GetById(playerActiveSpellId);
    }

    public List<PlayerActiveSpell> GetActiveSpells(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerActiveSpell>();

        return playerActiveSpellRepository.GetByPlayerId(playerId);
    }

    public PlayerActiveSpell GetByPlayerAndSpell(int playerId, int spellId)
    {
        if (playerId <= 0 || spellId <= 0)
            return null;

        return playerActiveSpellRepository.GetByPlayerAndSpell(playerId, spellId);
    }

    public PlayerActiveSpell GetByPlayerAndSlot(int playerId, int spellSlot)
    {
        if (playerId <= 0 ||
            spellSlot < 1 ||
            spellSlot > MaxActiveSpellSlots)
            return null;

        List<PlayerActiveSpell> activeSpells =
            playerActiveSpellRepository.GetByPlayerId(playerId);

        foreach (PlayerActiveSpell activeSpell in activeSpells)
        {
            if (activeSpell.SpellSlot == spellSlot)
                return activeSpell;
        }

        return null;
    }

    public int ActivateSpell(int playerId, int spellId, int spellSlot)
    {
        if (playerId <= 0 ||
            spellId <= 0 ||
            spellSlot < 1 ||
            spellSlot > MaxActiveSpellSlots)
            return 0;

        if (!playerHasSpellService.HasSpell(playerId, spellId))
            return 0;

        PlayerActiveSpell existingSpell =
            playerActiveSpellRepository.GetByPlayerAndSpell(playerId, spellId);

        if (existingSpell != null)
        {
            if (existingSpell.SpellSlot == spellSlot)
                return existingSpell.PlayerActiveSpellId;

            return 0;
        }

        PlayerActiveSpell slotSpell =
            GetByPlayerAndSlot(playerId, spellSlot);

        if (slotSpell != null)
        {
            slotSpell.SpellId = spellId;

            bool updated = playerActiveSpellRepository.Update(slotSpell);

            return updated ? slotSpell.PlayerActiveSpellId : 0;
        }

        PlayerActiveSpell playerActiveSpell = new PlayerActiveSpell
        {
            PlayerId = playerId,
            SpellId = spellId,
            SpellSlot = spellSlot
        };

        return playerActiveSpellRepository.Add(playerActiveSpell);
    }

    public bool DeactivateSpell(int playerActiveSpellId)
    {
        if (playerActiveSpellId <= 0)
            return false;

        return playerActiveSpellRepository.Delete(playerActiveSpellId);
    }

    public int DeactivateAllSpells(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return playerActiveSpellRepository.DeleteByPlayerId(playerId);
    }

    public bool IsSpellActive(int playerId, int spellId)
    {
        if (playerId <= 0 || spellId <= 0)
            return false;

        return playerActiveSpellRepository.Exists(playerId, spellId);
    }

    public int GetActiveSpellCount(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return playerActiveSpellRepository.CountByPlayerId(playerId);
    }
}