using UnityEngine;

public readonly struct BrickSetup
{
    public Sprite Sprite { get; }
    public Color Tint { get; }
    public Color HitFlashColor { get; }
    public int Score { get; }
    public bool IsIndestructible { get; }
    public int HitsToDestroy { get; }
    public bool CanDropItem => !IsIndestructible && HitsToDestroy == 1;

    private BrickSetup(Sprite sprite, Color tint, Color hitFlashColor, int score, bool isIndestructible, int hitsToDestroy)
    {
        Sprite = sprite;
        Tint = tint;
        HitFlashColor = hitFlashColor;
        Score = score;
        IsIndestructible = isIndestructible;
        HitsToDestroy = hitsToDestroy;
    }

    public static BrickSetup FromRow(BrickRowDefinition row, StageConfig config)
    {
        return new BrickSetup(row.sprite, Color.white, config.hitFlashColor, row.score, false, row.durability);
    }

    public static BrickSetup Gold(StageConfig config)
    {
        return new BrickSetup(config.goldBrickSprite, config.goldBrickTint, config.goldBrickHitFlashColor, 0, true, 0);
    }

    public BrickDurability CreateDurability()
    {
        return IsIndestructible ? BrickDurability.Indestructible() : new BrickDurability(HitsToDestroy);
    }
}
