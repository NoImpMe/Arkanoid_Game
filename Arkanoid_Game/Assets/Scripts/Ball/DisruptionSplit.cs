using UnityEngine;

public static class DisruptionSplit
{
    public static float AngleFromVertical(Vector2 velocity)
    {
        return Mathf.Atan2(velocity.x, Mathf.Abs(velocity.y)) * Mathf.Rad2Deg;
    }

    public static Vector2 Rotated(Vector2 velocity, float angleOffsetDegrees, float maxAngleDegrees)
    {
        float newAngle = Mathf.Clamp(AngleFromVertical(velocity) + angleOffsetDegrees, -maxAngleDegrees, maxAngleDegrees);
        Vector2 rotated = BallBounce.VelocityFromAngle(Mathf.Abs(velocity.y), newAngle);
        rotated.y *= Mathf.Sign(velocity.y);
        return rotated;
    }

    public static bool ShouldLoseLife(int activeBallCount)
    {
        return activeBallCount <= 0;
    }

    public static bool HasMultipleBalls(int activeBallCount)
    {
        return activeBallCount > 1;
    }
}
