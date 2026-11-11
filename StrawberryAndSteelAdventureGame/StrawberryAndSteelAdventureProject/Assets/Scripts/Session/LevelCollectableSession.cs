
public class LevelCollectableSession
{
    public int CoinCount { get; private set; }
    public int DiamondCount { get; private set; }

    public void AddCoin()
    {
        CoinCount++;
    }

    public void AddDiamond()
    {
        DiamondCount++;
    }

    public void Clear()
    {
        CoinCount = 0;
        DiamondCount = 0;
    }
}
