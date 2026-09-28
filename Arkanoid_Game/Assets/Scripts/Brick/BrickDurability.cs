using UnityEngine;

public class BrickDurability
{
    public int RemainingHits { get; private set; }
    public bool IsDestroyed => RemainingHits <= 0;

    public BrickDurability(int hitsToDestroy)
    {
        RemainingHits = hitsToDestroy;
    }

    public bool TakeHit()
    {
        RemainingHits = Mathf.Max(RemainingHits - 1, 0);
        return IsDestroyed;
    }
}
