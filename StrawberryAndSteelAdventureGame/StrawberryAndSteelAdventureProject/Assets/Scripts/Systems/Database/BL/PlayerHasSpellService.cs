using System.Collections.Generic;

public class PlayerHasSpellService
{
    private readonly PlayerHasSpellRepository playerHasSpellRepository;

    public PlayerHasSpellService(PlayerHasSpellRepository playerHasSpellRepository)
    {
        this.playerHasSpellRepository = playerHasSpellRepository;
    }

    public PlayerHasSpell GetById(int playerHasSpellId)
    {
        if (playerHasSpellId <= 0)
            return null;

        return playerHasSpellRepository.GetById(playerHasSpellId);
    }

    public PlayerHasSpell GetByPlayerAndSpell(int playerId, int spellId)
    {
        if (playerId <= 0 || spellId <= 0)
            return null;

        return playerHasSpellRepository.GetByPlayerAndSpell(playerId, spellId);
    }

    public List<PlayerHasSpell> GetPlayerSpells(int playerId)
    {
        if (playerId <= 0)
            return new List<PlayerHasSpell>();

        return playerHasSpellRepository.GetByPlayerId(playerId);
    }

    public int LearnSpell(int playerId, int spellId)
    {
        if (playerId <= 0 || spellId <= 0)
            return 0;

        if (playerHasSpellRepository.Exists(playerId, spellId))
            return 0;

        PlayerHasSpell playerHasSpell = new PlayerHasSpell
        {
            PlayerId = playerId,
            SpellId = spellId
        };

        return playerHasSpellRepository.Add(playerHasSpell);
    }

    public bool UpdatePlayerSpell(PlayerHasSpell playerHasSpell)
    {
        if (playerHasSpell == null ||
            playerHasSpell.PlayerHasSpellId <= 0 ||
            playerHasSpell.PlayerId <= 0 ||
            playerHasSpell.SpellId <= 0)
            return false;

        return playerHasSpellRepository.Update(playerHasSpell);
    }

    public bool ForgetSpell(int playerHasSpellId)
    {
        if (playerHasSpellId <= 0)
            return false;

        return playerHasSpellRepository.Delete(playerHasSpellId);
    }

    public int ForgetAllSpells(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return playerHasSpellRepository.DeleteByPlayerId(playerId);
    }

    public bool HasSpell(int playerId, int spellId)
    {
        if (playerId <= 0 || spellId <= 0)
            return false;

        return playerHasSpellRepository.Exists(playerId, spellId);
    }

    public int GetPlayerSpellCount(int playerId)
    {
        if (playerId <= 0)
            return 0;

        return playerHasSpellRepository.CountByPlayerId(playerId);
    }
}