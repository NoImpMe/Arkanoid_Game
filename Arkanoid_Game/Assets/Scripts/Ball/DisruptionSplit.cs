using UnityEngine;

public static class DisruptionSplit
{
    public static float AngleFromVertical(Vector2 velocity)
    {
        return Mathf.Atan2(velocity.x, Mathf.Abs(velocity.y)) * Mathf.Rad2Deg;
    }

    public static (float first, float second) SplitAngles(float originalAngle, float spreadDegrees, float maxAngleDegrees)
    {
        if (Exceeds(originalAngle + spreadDegrees, maxAngleDegrees))
        {
            return (originalAngle - spreadDegrees, originalAngle - 2f * spreadDegrees);
        }

        if (Exceeds(-(originalAngle - spreadDegrees), maxAngleDegrees))
        {
            return (originalAngle + spreadDegrees, originalAngle + 2f * spreadDegrees);
        }

        return (originalAngle + spreadDegrees, originalAngle - spreadDegrees);
    }

    public static Vector2 WithAngle(Vector2 velocity, float angleDegrees, float maxAngleDegrees)
    {
        float clampedAngle = Mathf.Clamp(angleDegrees, -maxAngleDegrees, maxAngleDegrees);
        Vector2 result = BallBounce.VelocityFromAngle(Mathf.Abs(velocity.y), clampedAngle);
        result.y *= Mathf.Sign(velocity.y);
        return result;
    }

    private static bool Exceeds(float angle, float maxAngleDegrees)
    {
        return angle > maxAngleDegrees && !Mathf.Approximately(angle, maxAngleDegrees);
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
