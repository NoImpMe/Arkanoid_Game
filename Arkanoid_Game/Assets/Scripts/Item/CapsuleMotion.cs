using UnityEngine;

public static class CapsuleMotion
{
    public static Vector2 Fall(Vector2 center, float fallSpeed, float deltaTime)
    {
        return center + Vector2.down * (fallSpeed * deltaTime);
    }

    public static Rect SweptArea(Rect before, Rect after)
    {
        return Rect.MinMaxRect(
            Mathf.Min(before.xMin, after.xMin),
            Mathf.Min(before.yMin, after.yMin),
            Mathf.Max(before.xMax, after.xMax),
            Mathf.Max(before.yMax, after.yMax));
    }

    public static bool IsCaught(Rect before, Rect after, Rect paddleArea)
    {
        return SweptArea(before, after).Overlaps(paddleArea);
    }

    public static bool HasLeftField(Rect capsuleArea, float deadZoneY)
    {
        return capsuleArea.yMax < deadZoneY;
    }
}
