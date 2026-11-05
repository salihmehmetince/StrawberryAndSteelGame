
using System;

public class GameSession
{
    public int CurrentPlayerId { get; private set; }

    public bool HasActivePlayer => CurrentPlayerId > 0;

    public void SetCurrentPlayer(int playerId)
    {
        if (playerId <= 0)
            throw new ArgumentException("Invalid PlayerId.");

        CurrentPlayerId = playerId;
    }

    public void ClearCurrentPlayer()
    {
        CurrentPlayerId = 0;
    }
}
