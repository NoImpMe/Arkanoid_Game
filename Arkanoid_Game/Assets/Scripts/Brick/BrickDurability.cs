using UnityEngine;

public class BrickDurability
{
    public int RemainingHits { get; private set; }
    public bool IsIndestructible { get; }
    public bool IsDestroyed => !IsIndestructible && RemainingHits <= 0;

    public BrickDurability(int hitsToDestroy)
    {
        RemainingHits = hitsToDestroy;
    }

    private BrickDurability()
    {
        IsIndestructible = true;
    }

    public static BrickDurability Indestructible()
    {
        return new BrickDurability();
    }

    public bool TakeHit()
    {
        if (IsIndestructible)
        {
            return false;
        }

        RemainingHits = Mathf.Max(RemainingHits - 1, 0);
        return IsDestroyed;
    }
}
