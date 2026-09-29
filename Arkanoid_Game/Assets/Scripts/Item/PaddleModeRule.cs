public static class PaddleModeRule
{
    public static PaddleMode AfterPickup(PaddleMode currentMode, ItemType pickedItem)
    {
        switch (pickedItem)
        {
            case ItemType.Lasers:
                return PaddleMode.Laser;
            case ItemType.Enlarge:
                return PaddleMode.Enlarge;
            case ItemType.Catch:
                return PaddleMode.Catch;
            default:
                return currentMode;
        }
    }

    public static PaddleMode AfterBallCaught(PaddleMode currentMode)
    {
        return currentMode == PaddleMode.Catch ? PaddleMode.None : currentMode;
    }

    public static bool HasDuration(PaddleMode mode)
    {
        return mode == PaddleMode.Laser || mode == PaddleMode.Enlarge;
    }

    public static float DurationFor(PaddleMode mode, float laserDurationSeconds, float enlargeDurationSeconds)
    {
        switch (mode)
        {
            case PaddleMode.Laser:
                return laserDurationSeconds;
            case PaddleMode.Enlarge:
                return enlargeDurationSeconds;
            default:
                return 0f;
        }
    }

    public static float WidthFor(PaddleMode mode, float normalWidth, float enlargeWidthMultiplier)
    {
        return mode == PaddleMode.Enlarge ? normalWidth * enlargeWidthMultiplier : normalWidth;
    }
}
