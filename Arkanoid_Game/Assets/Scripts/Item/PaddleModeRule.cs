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

    public static float WidthFor(PaddleMode mode, float normalWidth, float enlargeWidthMultiplier)
    {
        return mode == PaddleMode.Enlarge ? normalWidth * enlargeWidthMultiplier : normalWidth;
    }
}
